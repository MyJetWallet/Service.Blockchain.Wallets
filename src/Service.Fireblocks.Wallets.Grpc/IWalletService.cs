using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fireblocks.Wallets.Grpc.Models.UserWallets;

namespace Service.Fireblocks.Wallets.Grpc
{
    [ServiceContract]
    public interface IWalletService
    {
        [OperationContract]
        Task<GetUserWalletResponse> GetUserWalletAsync(GetUserWalletRequest request);
    }
}