using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Fireblocks.Api.Grpc;
using Service.Fireblocks.Wallets.Grpc;
using Service.Fireblocks.Wallets.Grpc.Models;
using Service.Fireblocks.Wallets.MyNoSql.AssetsMappings;
using Service.Fireblocks.Wallets.Postgres;
using Service.Fireblocks.Wallets.Postgres.Entities;
using Service.Fireblocks.Wallets.Settings;

namespace Service.Fireblocks.Wallets.Services
{
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IVaultAccountService _vaultAccountService;
        private readonly IMyNoSqlServerDataWriter<AssetMappingNoSql> _assetMappings;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public WalletService(ILogger<WalletService> logger,
            IVaultAccountService vaultAccountService,
            IMyNoSqlServerDataWriter<AssetMappingNoSql> assetMappings,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _logger = logger;
            this._vaultAccountService = vaultAccountService;
            this._assetMappings = assetMappings;
            this._dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task<GetUserWalletResponse> GetUserWalletAsync(GetUserWalletRequest request)
        {
            var asset = await _assetMappings.GetAsync(AssetMappingNoSql.GeneratePartitionKey(request.AssetId),
                AssetMappingNoSql.GenerateRowKey(request.AssetNetworkId));

            await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

            UserAddressEntity addressEntity = await context.VaultAddresses.FirstAsync(x => x.FireblocksAssetId == asset.AssetMapping.FireblocksAssetId &&
            x.UserId == request.UserId);

            if (addressEntity == null)
                switch (asset.AssetMapping.DepositType)
                {
                    case MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Broker:
                        {
                            var vaultAddress = await _vaultAccountService.CreateVaultAddressAsync(new Api.Grpc.Models.Addresses.CreateVaultAddressRequest
                            {
                                AssetId = asset.AssetMapping.FireblocksAssetId,
                                CustomerRefId = request.UserId,
                                Name = request.UserId,
                                VaultAccountId = asset.AssetMapping.ActiveDepositAddessVaultAccountId
                            });

                            addressEntity = new UserAddressEntity()
                            {
                                Address = vaultAddress.VaultAddress.Address,
                                AssetId = asset.AssetMapping.AssetId,
                                FireblocksAssetId = asset.AssetMapping.FireblocksAssetId,
                                UserId = request.UserId,
                                FireblocksVaultAccountId = asset.AssetMapping.ActiveDepositAddessVaultAccountId,
                                NetworkId = asset.AssetMapping.NetworkId,
                                Tag = vaultAddress.VaultAddress.Tag,
                                LegacyAddress = vaultAddress.VaultAddress.LegacyAddress,
                                Bip44AddressIndex = vaultAddress.VaultAddress.Bip44AddressIndex,
                                EnterpriseAddress = vaultAddress.VaultAddress.EnterpriseAddress
                            };

                            await context.VaultAddresses.Upsert(addressEntity).RunAsync();

                            break;
                        }
                    case MyJetWallet.Fireblocks.Domain.Models.AssetMappngs.DepositType.Intermediate:
                        {
                            var vault = await _vaultAccountService.CreateVaultAccountAsync(new Api.Grpc.Models.VaultAccounts.CreateVaultAccountRequest
                            {
                                AutoFuel = true,
                                Name = request.UserId,
                                CustomerRefId = request.UserId,
                                //We can hide it on ui of fireblocks
                                HiddenOnUI = false
                            });

                            var vaultAsset = await _vaultAccountService.CreateVaultAssetAsync(new Api.Grpc.Models.VaultAssets.CreateVaultAssetRequest
                            {
                                AsssetId = request.AssetId,
                                VaultAccountId = vault.VaultAccount.Id,
                            });

                            addressEntity = new UserAddressEntity()
                            {
                                Address = vaultAsset.VaultAddress.Address,
                                AssetId = asset.AssetMapping.AssetId,
                                FireblocksAssetId = asset.AssetMapping.FireblocksAssetId,
                                UserId = request.UserId,
                                FireblocksVaultAccountId = vault.VaultAccount.Id,
                                NetworkId = asset.AssetMapping.NetworkId,
                                Tag = vaultAsset.VaultAddress.Tag,
                                LegacyAddress = vaultAsset.VaultAddress.LegacyAddress,
                                Bip44AddressIndex = vaultAsset.VaultAddress.Bip44AddressIndex,
                                EnterpriseAddress = vaultAsset.VaultAddress.EnterpriseAddress
                            };

                            await using var transaction = context.Database.BeginTransaction();
                            await context.VaultAccounts.Upsert(vault.VaultAccount).RunAsync();
                            await context.VaultAddresses.Upsert(addressEntity).RunAsync();
                            await transaction.CommitAsync();

                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(asset.AssetMapping.DepositType), asset.AssetMapping.DepositType, null);
                }

            return new GetUserWalletResponse
            {
                AssetId = addressEntity.AssetId,
                AssetNetworkId = addressEntity.NetworkId,
                UserId = request.UserId,
                VaultAddress = new MyJetWallet.Fireblocks.Domain.Models.Addresses.VaultAddress
                {
                    Address = addressEntity.Address,
                    Bip44AddressIndex = addressEntity.Bip44AddressIndex,
                    EnterpriseAddress = addressEntity.EnterpriseAddress,
                    LegacyAddress = addressEntity.LegacyAddress,
                    Tag = addressEntity.Tag,
                }
            };
        }
    }
}
