using System.Runtime.Serialization;

namespace Service.Fireblocks.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserWalletRequest
    {
        [DataMember(Order = 1)]
        public string WalletId { get; set; }

        [DataMember(Order = 2)]
        public string AssetSymbol { get; set; }

        [DataMember(Order = 3)]
        public string AssetNetwork { get; set; }

        [DataMember(Order = 4)]
        public string BrokerId { get; set; }
    }
}