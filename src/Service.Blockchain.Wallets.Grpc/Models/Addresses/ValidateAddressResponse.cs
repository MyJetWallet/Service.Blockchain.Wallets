using Service.Blockchain.Wallets.Grpc.Models;
using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.Addresses
{
    [DataContract]
    public class ValidateAddressResponse
    {
        [DataMember(Order = 1)]
        public string Address { get; set; }

        [DataMember(Order = 2)]
        public bool IsValid { get; set; }

        [DataMember(Order = 3)]
        public ErrorResponse Error { get; set; }

        [DataMember(Order = 4)]
        public bool IsInternal { get; set; }
    }
}