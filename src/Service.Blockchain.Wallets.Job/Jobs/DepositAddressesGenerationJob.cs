﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Circle.Settings.Services;
using MyJetWallet.Domain.Assets;
using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.Blockchain.Wallets.Common.Settings;
using Service.Blockchain.Wallets.Job;
using Service.Blockchain.Wallets.MyNoSql.AssetsMappings;
using Service.Blockchain.Wallets.Postgres;
using Service.Blockchain.Wallets.Postgres.Entities;
using Service.Circle.Signer.Grpc;
using Service.Circle.Signer.Grpc.Models;
using Service.Fireblocks.Api.Grpc;

// ReSharper disable InconsistentLogPropertyNaming

namespace Service.Blockchain.Wallets.Job.Jobs
{
    public class DepositAddressesGenerationJob : IDisposable
    {
        private readonly ILogger<DepositAddressesGenerationJob> _logger;
        private readonly IAssetsDictionaryClient _assetsDictionaryClient;
        private readonly IAssetPaymentSettingsClient _assetPaymentSettingsClient;
        private readonly ICircleAssetSettingsService _circleAssetSettingsService;
        private readonly ICircleBlockchainMapper _blockchainMapper;
        private readonly ICircleDepositAddressService _circleDepositAddressService;
        private readonly IMyNoSqlServerDataReader<AssetMappingNoSql> _fireblocksAssetMapping;
        private readonly IVaultAccountService _vaultAccountService;
        private readonly SettingsModel _settingsModel;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly MyTaskTimer _timer;

        public DepositAddressesGenerationJob(
            ILogger<DepositAddressesGenerationJob> logger,
            IAssetsDictionaryClient assetsDictionaryClient,
            IAssetPaymentSettingsClient assetPaymentSettingsClient,
            ICircleAssetSettingsService circleAssetSettingsService,
            ICircleBlockchainMapper blockchainMapper,
            ICircleDepositAddressService circleDepositAddressService,
            IMyNoSqlServerDataReader<AssetMappingNoSql> fireblocksAssetMapping,
            IVaultAccountService vaultAccountService,
            SettingsModel settingsModel,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            _assetsDictionaryClient = assetsDictionaryClient;
            _assetPaymentSettingsClient = assetPaymentSettingsClient;
            _circleAssetSettingsService = circleAssetSettingsService;
            _blockchainMapper = blockchainMapper;
            _circleDepositAddressService = circleDepositAddressService;
            this._fireblocksAssetMapping = fireblocksAssetMapping;
            this._vaultAccountService = vaultAccountService;
            this._settingsModel = settingsModel;
            this._dbContextOptionsBuilder = dbContextOptionsBuilder;
            _timer = new MyTaskTimer(typeof(DepositAddressesGenerationJob),
                TimeSpan.FromSeconds(int.Parse(_settingsModel.GenerateAddressesIntervalSec)),
                logger, DoTime);
        }

        private async Task DoTime()
        {
            var maxCount = int.Parse(_settingsModel.PreGeneratedAddressesCount);
            var circleAssets = await _circleAssetSettingsService.GetAllAssetMapsAsync();
            var fireblocksAssets = _fireblocksAssetMapping.Get();

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

            //var getCountQuery = $@"SELECT 
            //                       addr.""{nameof(UserAddressEntity.AssetSymbol)}"", 
            //                       addr.""{nameof(UserAddressEntity.AssetNetwork)}"",
            //                       COUNT(*) as WalletCount 
            //                       FROM ""{DatabaseContext.Schema}"".{DatabaseContext.AddressesTableName} as addr 
            //                       WHERE addr.""{nameof(UserAddressEntity.WalletId)}"" is null
            //                       GROUP BY addr.""{nameof(UserAddressEntity.AssetSymbol)}"", addr.""{nameof(UserAddressEntity.AssetNetwork)}""";

            var aggregatedWalletCount = await context.VaultAddresses
                .Where(x => x.WalletId == null)
                .GroupBy(x => new { x.AssetSymbol, x.AssetNetwork })
                .Select(x => new { x.Key.AssetSymbol, x.Key.AssetNetwork, WalletCount = x.Count() })
                .ToListAsync();

            //var aggregatedWalletCount = await context.Database.GetDbConnection()
            //        .QueryAsync<AdressWalletGrouping>(getCountQuery);

            var walletCountDict = aggregatedWalletCount.ToDictionary(x => (x.AssetSymbol, x.AssetNetwork), y => y.WalletCount);

            foreach (var circleAsset in circleAssets)
            {
                var assetIdentity = new AssetIdentity
                {
                    BrokerId = circleAsset.BrokerId,
                    Symbol = circleAsset.AssetSymbol
                };

                var paymentSettings = _assetPaymentSettingsClient.GetAssetById(assetIdentity);
                if (paymentSettings?.Circle?.IsEnabledBlockchainDeposit != true)
                    continue;

                var asset = _assetsDictionaryClient.GetAssetById(assetIdentity);

                if (asset == null || !asset.IsEnabled)
                    continue;

                foreach (var blockchain in asset.DepositBlockchains)
                {
                    var circleBlockchain =
                        _blockchainMapper.BlockchainToCircleBlockchain(asset.BrokerId, blockchain);
                    if (circleBlockchain == null)
                        continue;

                    walletCountDict.TryGetValue((asset.Symbol, blockchain), out var entitiesCount);

                    if (entitiesCount < maxCount)
                    {
                        for (var i = 1; i <= maxCount - entitiesCount; i++)
                            try
                            {
                                var id = Guid.NewGuid().ToString();
                                //var label = $"PreGenerated-{id}";
                                var response = await _circleDepositAddressService.GenerateDepositAddress(
                                    new CreateCircleDepositAddressRequest
                                    {
                                        RequestGuid = id,
                                        BrokerId = asset.BrokerId,
                                        Asset = asset.Symbol,
                                        Blockchain = blockchain
                                    });

                                if (!response.IsSuccess)
                                {
                                    _logger.LogError(
                                        "Unable to pre-generate address for broker {broker}, asset {asset}, wallet id {walletId}, blockchain  {blockchain}: {error}",
                                        asset.BrokerId, asset.Symbol, circleAsset.CircleWalletId, blockchain,
                                        response.ErrorMessage);
                                    continue;
                                }

                                var addressEntity = new UserAddressEntity
                                {
                                    BrokerId = asset.BrokerId,
                                    CircleWalletId = circleAsset.CircleWalletId,
                                    AddressId = id,
                                    Address = response.Data.Address,
                                    AddressLowerCase = response.Data.Address.ToLowerInvariant(),
                                    AssetNetwork = blockchain,
                                    AssetSymbol = asset.Symbol,
                                    Bip44AddressIndex = 0,
                                    Integration = Domain.Models.BlockchainIntegration.Circle,
                                    Status = Domain.Models.AddressStatus.New,
                                };
                                await context.VaultAddresses.Upsert(addressEntity).RunAsync();
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Unable to pre-generate address for broker {broker}, asset {asset}, wallet id {walletId}, blockchain {blockchain}",
                                    asset.BrokerId, asset.Symbol, circleAsset.CircleWalletId, blockchain);
                            }

                        _logger.LogInformation(
                            "Pre-generated {count} addresses for broker {broker}, asset {asset}, wallet id {walletId}, blockchain {blockchain}",
                            maxCount - entitiesCount, asset.BrokerId, asset.Symbol, circleAsset.CircleWalletId,
                            blockchain);
                    }
                }
            }

            foreach (var fireblockAsset in fireblocksAssets)
            {
                var assetIdentity = new AssetIdentity
                {
                    BrokerId = MyJetWallet.Domain.DomainConstants.DefaultBroker,
                    Symbol = fireblockAsset.AssetMapping.AssetId
                };

                var paymentSettings = _assetPaymentSettingsClient.GetAssetById(assetIdentity);
                if (paymentSettings?.Fireblocks?.IsEnabledDeposit != true)
                    continue;

                var asset = _assetsDictionaryClient.GetAssetById(assetIdentity);

                if (asset == null || !asset.IsEnabled)
                    continue;

                foreach (var blockchain in asset.DepositBlockchains)
                {
                    walletCountDict.TryGetValue((asset.Symbol, blockchain), out var entitiesCount);

                    if (entitiesCount < maxCount)
                    {
                        for (var i = 1; i <= maxCount - entitiesCount; i++)
                            try
                            {
                                var pregeneratedId = Guid.NewGuid().ToString();
                                //var label = $"PreGenerated-{id}";
                                switch (fireblockAsset.AssetMapping.DepositType)
                                {
                                    case MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Broker:
                                        {
                                            var vaultAddress = await _vaultAccountService.CreateVaultAddressAsync(new()
                                            {
                                                AssetId = fireblockAsset.AssetMapping.FireblocksAssetId,
                                                CustomerRefId = pregeneratedId,
                                                Name = pregeneratedId,
                                                VaultAccountId = fireblockAsset.AssetMapping.ActiveDepositAddessVaultAccountId
                                            });

                                            if (vaultAddress.Error != null)
                                            {
                                                _logger.LogError("Can't create fireblocks wallet @{context}", new { });
                                                continue;
                                            }

                                            var addressEntity = new UserAddressEntity
                                            {
                                                Address = vaultAddress.VaultAddress.Address,
                                                AddressLowerCase = vaultAddress.VaultAddress.Address.ToLowerInvariant(),
                                                AssetNetwork = blockchain,
                                                AssetSymbol = asset.Symbol,
                                                Bip44AddressIndex = vaultAddress.VaultAddress.Bip44AddressIndex,
                                                AddressId = pregeneratedId,
                                                EnterpriseAddress = vaultAddress.VaultAddress.EnterpriseAddress,
                                                FireblocksAssetId = fireblockAsset.AssetMapping.FireblocksAssetId,
                                                FireblocksVaultAccountId = fireblockAsset.AssetMapping.ActiveDepositAddessVaultAccountId,
                                                Integration = Domain.Models.BlockchainIntegration.Fireblocks,
                                                IsActive = true,
                                                Status = Domain.Models.AddressStatus.New,
                                                Tag = vaultAddress.VaultAddress.Tag
                                            };

                                            await context.VaultAddresses.Upsert(addressEntity).RunAsync();

                                            break;
                                        }
                                    case MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Intermediate:
                                        {
                                            var vault = await _vaultAccountService.CreateVaultAccountAsync(new()
                                            {
                                                AutoFuel = true,
                                                Name = pregeneratedId,
                                                CustomerRefId = pregeneratedId,
                                                //We can hide it on ui of fireblocks
                                                HiddenOnUI = false
                                            });

                                            if (vault.Error != null)
                                            {
                                                _logger.LogError("Can't create fireblocks wallet @{context}", new { });
                                                continue;
                                            }

                                            VaultAddress vaultAddress = null;

                                            var vaultAsset = await _vaultAccountService.CreateVaultAssetAsync(new()
                                            {
                                                AsssetId = fireblockAsset.AssetMapping.FireblocksAssetId,
                                                VaultAccountId = vault.VaultAccount.Id,
                                            });

                                            vaultAddress = vaultAsset.VaultAddress;

                                            if (vaultAddress == null)
                                            {
                                                var existing = await _vaultAccountService.GetVaultAddressAsync(new()
                                                {
                                                    AssetId = fireblockAsset.AssetMapping.FireblocksAssetId,
                                                    VaultAccountId = vault.VaultAccount.Id,
                                                });

                                                vaultAddress = existing?.VaultAddress?.FirstOrDefault();
                                            }

                                            if (vaultAddress == null)
                                            {
                                                _logger.LogError("Can't create fireblocks wallet @{context}", new { });
                                                continue;
                                            }


                                            var addressEntity = new UserAddressEntity
                                            {
                                                Address = vaultAddress.Address,
                                                AddressLowerCase = vaultAddress.Address.ToLowerInvariant(),
                                                AssetNetwork = blockchain,
                                                AssetSymbol = asset.Symbol,
                                                Bip44AddressIndex = vaultAddress.Bip44AddressIndex,
                                                AddressId = pregeneratedId,
                                                EnterpriseAddress = vaultAddress.EnterpriseAddress,
                                                FireblocksAssetId = fireblockAsset.AssetMapping.FireblocksAssetId,
                                                FireblocksVaultAccountId = fireblockAsset.AssetMapping.ActiveDepositAddessVaultAccountId,
                                                Integration = Domain.Models.BlockchainIntegration.Fireblocks,
                                                IsActive = true,
                                                Status = Domain.Models.AddressStatus.New,
                                                Tag = vaultAddress.Tag
                                            };

                                            await using var transaction = context.Database.BeginTransaction();
                                            await context.VaultAccounts.Upsert(vault.VaultAccount).RunAsync();
                                            await context.VaultAddresses.Upsert(addressEntity).RunAsync();
                                            await transaction.CommitAsync();

                                            break;
                                        }
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(fireblockAsset.AssetMapping.DepositType), fireblockAsset.AssetMapping.DepositType, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Unable to pre-generate address @{context}", new
                                    {
                                        asset.BrokerId,
                                        asset.Symbol,
                                        fireblockAsset.AssetMapping,
                                    });
                            }

                        _logger.LogInformation(
                            "Pre-generated addresses @{context}", new
                            {
                                Count = maxCount - entitiesCount,
                                asset.BrokerId,
                                asset.Symbol,
                                fireblockAsset.AssetMapping.NetworkId,
                            });
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}