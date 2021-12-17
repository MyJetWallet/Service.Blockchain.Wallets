using System.Runtime.Serialization;

namespace Service.Fireblocks.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserWalletRequest
    {
        [DataMember(Order = 1)]
        public string UserId { get; set; }

        [DataMember(Order = 2)]
        public string AssetId { get; set; }

        [DataMember(Order = 3)]
        public string AssetNetworkId { get; set; }
    }
}