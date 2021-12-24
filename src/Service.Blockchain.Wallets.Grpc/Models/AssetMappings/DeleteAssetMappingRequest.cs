using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.AssetMappings
{
    [DataContract]
    public class DeleteAssetMappingRequest
    {
        [DataMember(Order = 2)]
        public string AssetId { get; set; }

        [DataMember(Order = 1)]
        public string AssetNetworkId { get; set; }
    }
}