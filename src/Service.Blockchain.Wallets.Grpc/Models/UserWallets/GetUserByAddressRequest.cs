using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.UserWallets
{
    [DataContract]
    public class GetUserByAddressRequest
    {
        [DataMember(Order = 1)]
        public AddressAndTag[] Addresses { get; set; }

        [DataContract]
        public class AddressAndTag
        {
            [DataMember(Order = 1)]
            public string Address { get; set; }

            [DataMember(Order = 2)]
            public string Tag { get; set; }
        }
    }
}