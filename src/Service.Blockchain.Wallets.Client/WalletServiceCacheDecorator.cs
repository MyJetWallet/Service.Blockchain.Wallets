﻿using MyNoSqlServer.Abstractions;
using Service.Blockchain.Wallets.Grpc;
using Service.Blockchain.Wallets.Grpc.Models.UserWallets;
using Service.Blockchain.Wallets.MyNoSql.Addresses;
using System.Threading.Tasks;

namespace Service.Blockchain.Wallets.Client
{
    public class WalletServiceCacheDecorator : IWalletService
    {
        private readonly IWalletService _service;
        private readonly IMyNoSqlServerDataReader<VaultAddressNoSql> _dataReader;

        public WalletServiceCacheDecorator(IWalletService service, IMyNoSqlServerDataReader<VaultAddressNoSql> dataReader)
        {
            this._service = service;
            this._dataReader = dataReader;
        }
        public async Task<GetUserWalletResponse> GetUserWalletAsync(GetUserWalletRequest request)
        {
            var cache = _dataReader.Get(VaultAddressNoSql.GeneratePartitionKey(request.WalletId), 
                VaultAddressNoSql.GenerateRowKey(request.AssetSymbol, request.AssetNetwork));

            if (cache != null)
            {
                return new GetUserWalletResponse
                {
                    AssetId = cache.AssetSymbol,
                    AssetNetworkId = cache.AssetNetwork,
                    UserId = cache.WalletId,
                    VaultAddress = cache.Address
                };
            }

            var result = await _service.GetUserWalletAsync(request);

            return result;
        }
    }
}