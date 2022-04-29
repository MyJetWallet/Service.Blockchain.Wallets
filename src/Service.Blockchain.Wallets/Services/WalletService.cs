using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyJetWallet.Domain.Assets;
using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyNoSqlServer.Abstractions;
using Service.AssetsDictionary.Client;
using Service.Fireblocks.Api.Grpc;
using Service.Blockchain.Wallets.Domain.Models;
using Service.Blockchain.Wallets.Grpc;
using Service.Blockchain.Wallets.Grpc.Models.UserWallets;
using Service.Blockchain.Wallets.MyNoSql.Addresses;
using Service.Blockchain.Wallets.MyNoSql.AssetsMappings;
using Service.Blockchain.Wallets.Postgres;
using Service.Blockchain.Wallets.Postgres.Entities;
using MyJetWallet.Circle.Settings.Services;
using Service.Circle.Signer.Grpc;
using Dapper;
using System.Text;
using System.Dynamic;
using System.Collections.Generic;
using Service.Blockchain.Wallets.Grpc.Models.Addresses;
using static Service.Blockchain.Wallets.Grpc.Models.UserWallets.GetUserByAddressResponse;
using MyJetWallet.Sdk.Service;
using MyJetWallet.ApiSecurityManager.AsymmetricEncryption;
using MyJetWallet.ApiSecurityManager.ApiKeys;
using MyJetWallet.ApiSecurityManager.WalletSignature;

namespace Service.Blockchain.Wallets.Services
{
    public class WalletService : IWalletService
    {
        private readonly ILogger<WalletService> _logger;
        private readonly IVaultAccountService _vaultAccountService;
        private readonly IMyNoSqlServerDataWriter<VaultAddressNoSql> _addressCache;
        private readonly IMyNoSqlServerDataWriter<AssetMappingNoSql> _assetMappings;
        private readonly IAssetsDictionaryClient _assetsDictionaryClient;
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;
        private readonly IAsymmetricEncryptionService _asymmetricEncryptionService;
        private readonly IApiKeyStorage _apiKeyStorage;
        private readonly IPaymentMethodsClient _paymentMethodsClient;

        public WalletService(ILogger<WalletService> logger,
            IVaultAccountService vaultAccountService,
            IMyNoSqlServerDataWriter<VaultAddressNoSql> addressCache,
            IMyNoSqlServerDataWriter<AssetMappingNoSql> assetMappings,
            IAssetsDictionaryClient assetsDictionaryClient,
            DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder,
            IAsymmetricEncryptionService asymmetricEncryptionService,
            IApiKeyStorage apiKeyStorage,
            IPaymentMethodsClient paymentMethodsClient)
        {
            _logger = logger;
            _vaultAccountService = vaultAccountService;
            _addressCache = addressCache;
            _assetMappings = assetMappings;
            _assetsDictionaryClient = assetsDictionaryClient;
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
            _asymmetricEncryptionService = asymmetricEncryptionService;
            _apiKeyStorage = apiKeyStorage;
            _paymentMethodsClient = paymentMethodsClient;
        }

        public async Task<GetUserWalletResponse> GetUserWalletAsync(GetUserWalletRequest request)
        {
            _logger.LogInformation("Get user wallet {context}", request.ToJson());

            var assetIdentity = new AssetIdentity()
            {
                BrokerId = request.BrokerId,
                Symbol = request.AssetSymbol
            };

            try
            {
                var asset = _assetsDictionaryClient.GetAssetById(assetIdentity);

                if (asset == null)
                    return GetUserWalletResponse.CreateErrorResponse("Asset is not found", Grpc.Models.ErrorCode.AssetDoNotFound);

                if (!asset.IsEnabled)
                    return GetUserWalletResponse.CreateErrorResponse("Asset is disabled", Grpc.Models.ErrorCode.AssetIsDisabled);

                var blockchain = request.AssetNetwork;
                if (blockchain == null)
                {
                    if (asset.DepositBlockchains.Count != 1)
                    {
                        return GetUserWalletResponse.CreateErrorResponse("Blockchain is not configured",
                            Grpc.Models.ErrorCode.BlockchainIsNotConfigured);
                    }

                    blockchain = asset.DepositBlockchains.First();
                }
                else
                {
                    if (!asset.DepositBlockchains.Contains(blockchain))
                    {
                        return GetUserWalletResponse.CreateErrorResponse("Blockchain is not supported",
                            Grpc.Models.ErrorCode.BlockchainIsNotSupported);
                    }
                }

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);

                UserAddressEntity addressEntity = await context.VaultAddresses.FirstOrDefaultAsync(x =>
                x.AssetNetwork == request.AssetNetwork &&
                x.AssetSymbol == request.AssetSymbol &&
                x.WalletId == request.WalletId);

                if (addressEntity == null)
                {
                    var assignQuery = $@"UPDATE ""{DatabaseContext.Schema}"".{DatabaseContext.AddressesTableName} as upd
                                      SET ""{nameof(UserAddressEntity.WalletId)}"" = @WalletId, 
                                      ""{nameof(UserAddressEntity.ClientId)}"" = @ClientId, 
                                      ""{nameof(UserAddressEntity.BrokerId)}"" = @BrokerId, 
                                      ""{nameof(UserAddressEntity.Status)}"" = @Status, 
                                      ""{nameof(UserAddressEntity.UpdatedAt)}"" = @UpdatedAt 
                                      WHERE upd.""{nameof(UserAddressEntity.AddressId)}"" = (SELECT addr.""{nameof(UserAddressEntity.AddressId)}""
                                      FROM ""{DatabaseContext.Schema}"".{DatabaseContext.AddressesTableName} as addr 
                                      WHERE addr.""{nameof(UserAddressEntity.WalletId)}"" is null and
                                            addr.""{nameof(UserAddressEntity.AssetSymbol)}"" = @AssetSymbol and
                                            addr.""{nameof(UserAddressEntity.AssetNetwork)}"" = @AssetNetwork
                                      ORDER BY addr.""{nameof(UserAddressEntity.AddressId)}""
                                      LIMIT 1
                                      FOR UPDATE SKIP LOCKED)
                                      RETURNING *;";

                    var addressEntities = await context.Database.GetDbConnection()
                            .QueryAsync<UserAddressEntity>(assignQuery, new
                            {
                                WalletId = request.WalletId,
                                ClientId = request.ClientId,
                                BrokerId = request.BrokerId,
                                AssetSymbol = request.AssetSymbol,
                                AssetNetwork = request.AssetNetwork,
                                Status = AddressStatus.WaitingForSignature,
                                UpdatedAt = DateTime.UtcNow,
                            });

                    //ERROR LOG: !!
                    if (addressEntities.Count() != 1)
                    {
                        _logger.LogError("Can't get address for {context}", new
                        {
                            Request = request,
                            AddressEntities = addressEntities
                        }.ToJson());

                        return GetUserWalletResponse.CreateErrorResponse("Please, wait for address to be generated",
                            Grpc.Models.ErrorCode.AddressPoolIsEmpty); ;
                    }

                    addressEntity = addressEntities.First();
                }

                if (addressEntity.Status == AddressStatus.WaitingForSignature)
                {

                    var privateKey = _apiKeyStorage.Get(Program.Settings.WalletSignaturePrivateApiKeyId);

                    if (privateKey == null)
                    {
                        _logger.LogError("Can't get/assign address for user, NO PRIVATE KEY FOR SIGNATURE: {context}", request.ToJson());

                        return new GetUserWalletResponse
                        {
                            Error = new Grpc.Models.ErrorResponse
                            {
                                Error = "NO PRIVATE KEY FOR SIGNATURE",
                                ErrorCode = Grpc.Models.ErrorCode.NoKey
                            }
                        };
                    }

                    addressEntity.SignatureSetUnixTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    var signatureComponents = WalletSignatureComponents.Create(
                        request.WalletId,
                        request.ClientId,
                        request.BrokerId,
                        request.AssetSymbol,
                        request.AssetNetwork,
                        addressEntity.Address,
                        addressEntity.Tag,
                        addressEntity.SignatureSetUnixTime);

                    var signatureContent = signatureComponents.GetSignatureContent();

                    var signature = _asymmetricEncryptionService.Sign(
                        signatureContent,
                        AsymmetricEncryptionUtils.ReadPrivateKeyFromPem(privateKey.PrivateKeyValue));

                    addressEntity.SigningKeyId = Program.Settings.WalletSignaturePrivateApiKeyId;
                    addressEntity.Signature = signature;
                    addressEntity.Status = AddressStatus.Assigned;
                    addressEntity.UpdatedAt = DateTime.UtcNow;
                    context.VaultAddresses.Update(addressEntity);

                    await context.SaveChangesAsync();
                }

                await _addressCache.InsertOrReplaceAsync(
                    VaultAddressNoSql.Create(
                        addressEntity.WalletId,
                        addressEntity.AssetSymbol,
                        addressEntity.AssetNetwork,
                        MaptToDomain(addressEntity),
                        addressEntity.ClientId,
                        addressEntity.BrokerId,
                        addressEntity.SignatureSetUnixTime,
                        addressEntity.SigningKeyId,
                        addressEntity.Signature));

                return new GetUserWalletResponse
                {
                    AssetId = addressEntity.AssetSymbol,
                    AssetNetworkId = addressEntity.AssetNetwork,
                    UserId = request.WalletId,
                    VaultAddress = MaptToDomain(addressEntity),
                    BrokerId = addressEntity.BrokerId,
                    ClientId = addressEntity.ClientId,
                    Signature = addressEntity.Signature,
                    SignatureIssuedAt = addressEntity.SignatureSetUnixTime,
                    SigningKeyId = addressEntity.SigningKeyId,
                };

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Can't get/assign address for user: {context}", request.ToJson());

                return new GetUserWalletResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
        }

        public async Task<GetUserByAddressResponse> GetUserByAddressAsync(GetUserByAddressRequest request)
        {
            _logger.LogInformation("Get user address {context}", request.ToJson());

            try
            {
                var tuples = request.Addresses.Select(x => new
                {
                    Address = x.Address.ToLowerInvariant(),
                    Tag = x.Tag ?? String.Empty,
                }).ToArray();


                //var useTags = tuples.Any(x => !string.IsNullOrEmpty(x.Tag));

                var builder = new StringBuilder();
                var parameter = (new ExpandoObject()) as IDictionary<string, Object>;

                for (var i = 0; i < tuples.Count(); i++)
                {
                    //if (useTags)
                    //{
                    builder.Append($"(@Address{i}, @Tag{i}),");
                    parameter.Add($"Address{i}", tuples[i].Address);
                    parameter.Add($"Tag{i}", tuples[i].Tag);
                    //}
                    //else
                    //{
                    //    builder.Append($"(@Address{i}),");
                    //    parameter.Add($"Address{i}", tuples[i].Address);
                    //}
                }

                builder.Remove(builder.Length - 1, 1);

                string assignQuery;

                //if (useTags)
                //{
                assignQuery = $@"SELECT * FROM ""{DatabaseContext.Schema}"".{DatabaseContext.AddressesTableName} 
                                  WHERE (""{nameof(UserAddressEntity.AddressLowerCase)}"", ""{nameof(UserAddressEntity.Tag)}"") in ({builder})";
                //}
                //else
                //{
                //    assignQuery = $@"SELECT * FROM ""{DatabaseContext.Schema}"".{DatabaseContext.AddressesTableName} 
                //                  WHERE (""{nameof(UserAddressEntity.AddressLowerCase)}"") in ({builder})";
                //}

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var addressEntities = await context.Database.GetDbConnection()
                        .QueryAsync<UserAddressEntity>(assignQuery, (object)parameter);

                return new GetUserByAddressResponse
                {
                    Users = addressEntities.Select(x => new UserForAddress
                    {
                        Address = x.AddressLowerCase,
                        BrokerId = x.BrokerId,
                        ClientId = x.ClientId,
                        Tag = x.Tag,
                        WalletId = x.WalletId
                    }).ToArray(),
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Can't get user for address {context}", request.ToJson());
                return new GetUserByAddressResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
        }

        public async Task<ValidateAddressResponse> ValidateAddressAsync(ValidateAddressRequest request)
        {
            _logger.LogInformation("Validate address {context}", request.ToJson());

            var assetIdentity = new AssetIdentity()
            {
                BrokerId = request.BrokerId,
                Symbol = request.AssetSymbol
            };

            try
            {
                var paymentSettings = _paymentMethodsClient.GetMethodsForAsset(assetIdentity.Symbol);

                var asset = _assetsDictionaryClient.GetAssetById(assetIdentity);

                if (asset == null)
                    return new ValidateAddressResponse()
                    {
                        Error = new Grpc.Models.ErrorResponse
                        {
                            Error = "Asset is not found",
                            ErrorCode = Grpc.Models.ErrorCode.AssetDoNotFound,
                        }
                    };

                if (!asset.IsEnabled)
                    return new ValidateAddressResponse()
                    {
                        Error = new Grpc.Models.ErrorResponse
                        {
                            Error = "Asset is disabled",
                            ErrorCode = Grpc.Models.ErrorCode.AssetIsDisabled,
                        }
                    };

                await using var context = new DatabaseContext(_dbContextOptionsBuilder.Options);
                var addressLowCase = request.Address.ToLowerInvariant();
                var addressEntities = await context.VaultAddresses.FirstOrDefaultAsync(x => x.AddressLowerCase == addressLowCase);

                if (addressEntities != null)
                {
                    return new ValidateAddressResponse()
                    {
                        Address = addressEntities.Address,
                        IsValid = true,
                        IsInternal = true,
                    };
                }

                if (paymentSettings?.Any(x => x.WithdrawalMethods.FireblocksBlockchain) ?? false)
                {
                    var assetMapping = await _assetMappings.GetAsync(AssetMappingNoSql.GeneratePartitionKey(asset.Symbol),
                        AssetMappingNoSql.GenerateRowKey(request.AssetNetwork));

                    if (assetMapping == null)
                    {
                        return new ValidateAddressResponse()
                        {
                            Error = new Grpc.Models.ErrorResponse
                            {
                                Error = "Asset is not found",
                                ErrorCode = Grpc.Models.ErrorCode.AssetDoNotFound,
                            }
                        };
                    }

                    var validationResponse = await _vaultAccountService.ValidateAddressAsync(new Fireblocks.Api.Grpc.Models.Addresses.ValidateAddressRequest
                    {
                        Address = request.Address,
                        AssetId = assetMapping.AssetMapping.FireblocksAssetId
                    });

                    return new ValidateAddressResponse
                    {
                        Address = request.Address,
                        IsValid = validationResponse.IsValid,
                        IsInternal = false,
                    };
                }
                else if (paymentSettings?.Any(x => x.WithdrawalMethods.CircleBlockchain) ?? false)
                {
                    //TODO:
                    _logger.LogInformation("Can't validate circle address", request.ToJson());
                }

                return new ValidateAddressResponse
                {
                    Address = request.Address,
                    IsValid = true,
                    IsInternal = false,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Can't get user for address {context}", request.ToJson());
                return new ValidateAddressResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
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


