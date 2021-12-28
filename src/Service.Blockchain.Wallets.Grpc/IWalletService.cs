using System.ServiceModel;
using System.Threading.Tasks;
using Service.Blockchain.Wallets.Grpc.Models.UserWallets;

namespace Service.Blockchain.Wallets.Grpc
{
    [ServiceContract]
    public interface IWalletService
    {
        [OperationContract]
        Task<GetUserWalletResponse> GetUserWalletAsync(GetUserWalletRequest request);

        [OperationContract]
        Task<GetUserByAddressResponse> GetUserByAddressAsync(GetUserByAddressRequest request);
    }
}