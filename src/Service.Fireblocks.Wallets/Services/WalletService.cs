using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Fireblocks.Domain.Models.Addresses;
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

                            addressEntity = MapToEntity(request, asset, vaultAddress.VaultAddress);

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

                            addressEntity = MapToEntity(request, asset, vaultAsset.VaultAddress);

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
                VaultAddress = MaptToDomain(addressEntity)
            };
        }

        private static UserAddressEntity MapToEntity(GetUserWalletRequest request, AssetMappingNoSql asset, VaultAddress vaultAddress)
        {
            return new UserAddressEntity()
            {
                Address = vaultAddress.Address,
                AssetId = asset.AssetMapping.AssetId,
                FireblocksAssetId = asset.AssetMapping.FireblocksAssetId,
                UserId = request.UserId,
                FireblocksVaultAccountId = asset.AssetMapping.ActiveDepositAddessVaultAccountId,
                NetworkId = asset.AssetMapping.NetworkId,
                Tag = vaultAddress.Tag,
                LegacyAddress = vaultAddress.LegacyAddress,
                Bip44AddressIndex = vaultAddress.Bip44AddressIndex,
                EnterpriseAddress = vaultAddress.EnterpriseAddress
            };
        }

        private static MyJetWallet.Fireblocks.Domain.Models.Addresses.VaultAddress MaptToDomain(UserAddressEntity userAddressEntity)
        {
            return new MyJetWallet.Fireblocks.Domain.Models.Addresses.VaultAddress
            {
                Address = userAddressEntity.Address,
                Bip44AddressIndex = userAddressEntity.Bip44AddressIndex,
                EnterpriseAddress = userAddressEntity.EnterpriseAddress,
                LegacyAddress = userAddressEntity.LegacyAddress,
                Tag = userAddressEntity.Tag,
            };
        }
    }
}
