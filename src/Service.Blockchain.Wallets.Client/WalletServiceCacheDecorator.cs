using MyNoSqlServer.Abstractions;
using Service.Blockchain.Wallets.Grpc;
using Service.Blockchain.Wallets.Grpc.Models.Addresses;
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

        public Task<GetUserByAddressResponse> GetUserByAddressAsync(GetUserByAddressRequest request)
        {
            return _service.GetUserByAddressAsync(request);
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
                    VaultAddress = cache.Address,
                    SigningKeyId = cache.SigningKeyId,
                    Signature= cache.Signature,
                    BrokerId = cache.BrokerId,
                    ClientId = cache.ClientId,
                    CreatedAt = cache.CreatedAt,
                };
            }

            var result = await _service.GetUserWalletAsync(request);

            return result;
        }

        public Task<ValidateAddressResponse> ValidateAddressAsync(ValidateAddressRequest request)
        {
            return _service.ValidateAddressAsync(request);
        }
    }
}
