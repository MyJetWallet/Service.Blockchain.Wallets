using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Fireblocks.Api.Grpc;
using Service.Blockchain.Wallets.Grpc;
using Service.Blockchain.Wallets.Grpc.Models.AssetMappings;
using Service.Blockchain.Wallets.MyNoSql.AssetsMappings;
using MyJetWallet.Sdk.Service;

namespace Service.Blockchain.Wallets.Services
{
    public class AssetMappingService : IAssetMappingService
    {
        private readonly ILogger<AssetMappingService> _logger;
        private readonly IMyNoSqlServerDataWriter<AssetMappingNoSql> _assetMappings;

        public AssetMappingService(ILogger<AssetMappingService> logger,
            IMyNoSqlServerDataWriter<AssetMappingNoSql> assetMappings)
        {
            _logger = logger;
            this._assetMappings = assetMappings;
        }

        public async Task<DeleteAssetMappingResponse> DeleteAssetMappingAsync(DeleteAssetMappingRequest request)
        {
            try
            {
                _logger.LogInformation("Deleting asset mapping {context}", request.ToJson());

                var mapping = await _assetMappings.DeleteAsync(AssetMappingNoSql.GeneratePartitionKey(request.AssetId),
                    AssetMappingNoSql.GeneratePartitionKey(request.AssetNetworkId));

                return new DeleteAssetMappingResponse { };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during AssetMapping delete: @{context}", request.ToJson());
                return new DeleteAssetMappingResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
        }

        public async Task<GetAssetMappingResponse> GetAssetMappingAsync(GetAssetMappingRequest request)
        {
            try
            {
                _logger.LogInformation("Get asset mapping {context}", request.ToJson());

                var mapping = await _assetMappings.GetAsync(AssetMappingNoSql.GeneratePartitionKey(request.AssetId),
                    AssetMappingNoSql.GeneratePartitionKey(request.AssetNetworkId));

                return new GetAssetMappingResponse
                {
                    AssetMapping = mapping?.AssetMapping
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during AssetMapping get: @{context}", request.ToJson());
                return new GetAssetMappingResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
        }

        public async Task<UpsertAssetMappingResponse> UpsertAssetMappingAsync(UpsertAssetMappingRequest request)
        {
            try
            {
                _logger.LogInformation("Upsert asset mapping {context}", request.ToJson());

                await _assetMappings.InsertOrReplaceAsync(AssetMappingNoSql.Create(request.AssetMapping));

                return new UpsertAssetMappingResponse
                {
                    AssetMapping = request.AssetMapping
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during AssetMapping upsert: @{context}", request.ToJson());
                return new UpsertAssetMappingResponse
                {
                    Error = new Grpc.Models.ErrorResponse
                    {
                        Error = e.Message,
                        ErrorCode = Grpc.Models.ErrorCode.Unknown
                    }
                };
            }
        }
    }
}
