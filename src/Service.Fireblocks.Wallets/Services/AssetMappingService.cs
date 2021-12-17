﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyNoSqlServer.Abstractions;
using Service.Fireblocks.Api.Grpc;
using Service.Fireblocks.Wallets.Grpc;
using Service.Fireblocks.Wallets.Grpc.Models.AssetMappings;
using Service.Fireblocks.Wallets.MyNoSql.AssetsMappings;

namespace Service.Fireblocks.Wallets.Services
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

        public async Task<GetAssetMappingResponse> GetAssetMappingAsync(GetAssetMappingRequest request)
        {
            try
            {
                var mapping = await _assetMappings.GetAsync(AssetMappingNoSql.GeneratePartitionKey(request.AssetId),
                    AssetMappingNoSql.GeneratePartitionKey(request.AssetNetworkId));

                return new GetAssetMappingResponse
                {
                    AssetMapping = mapping?.AssetMapping
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during AssetMapping get: @{context}", request);
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
                await _assetMappings.InsertOrReplaceAsync(AssetMappingNoSql.Create(request.AssetMapping));

                return new UpsertAssetMappingResponse
                {
                    AssetMapping = request.AssetMapping
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error during AssetMapping upsert: @{context}", request);
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
