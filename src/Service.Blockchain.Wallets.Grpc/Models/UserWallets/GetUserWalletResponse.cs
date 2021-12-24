using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserWalletResponse
    {
        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string AssetId { get; set; }

        [DataMember(Order = 3)]
        public string AssetNetworkId { get; set; }

        [DataMember(Order = 4)]
        public VaultAddress VaultAddress { get; set; }

        [DataMember(Order = 99)]
        public ErrorResponse Error { get; set; }

        public static GetUserWalletResponse CreateErrorResponse(string message, ErrorCode errorCode)
        {
            return new GetUserWalletResponse
            {
                Error = new ErrorResponse
                {
                    Error = message,
                    ErrorCode = errorCode
                }
            };
        }
    }
}