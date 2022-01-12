using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.Addresses
{
    [DataContract]
    public class ValidateAddressRequest
    {
        [DataMember(Order = 1)]
        public string Address { get; set; }

        [DataMember(Order = 2)]
        public string AssetSymbol { get; set; }

        [DataMember(Order = 3)]
        public string AssetNetwork { get; set; }

        [DataMember(Order = 4)]
        public string BrokerId { get; set; }
    }
}