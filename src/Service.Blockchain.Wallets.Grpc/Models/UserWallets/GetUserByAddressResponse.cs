using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserByAddressResponse
    {
        [DataMember(Order = 1)]
        public UserForAddress[] Users { get; set; }

        [DataMember(Order = 2)]
        public ErrorResponse Error { get; set; }

        [DataContract]
        public class UserForAddress
        {
            [DataMember(Order = 1)]
            public string Address { get; set; }

            [DataMember(Order = 2)]
            public string Tag { get; set; }

            [DataMember(Order = 3)]
            public string WalletId { get; set; }

            [DataMember(Order = 5)]
            public string BrokerId { get; set; }

            [DataMember(Order = 6)]
            public string ClientId { get; set; }
        }
    }
}