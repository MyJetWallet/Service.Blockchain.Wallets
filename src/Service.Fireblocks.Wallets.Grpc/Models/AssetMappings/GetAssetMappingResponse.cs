using MyJetWallet.Fireblocks.Domain.Models.AssetMappngs;
using System.Runtime.Serialization;

namespace Service.Fireblocks.Wallets.Grpc.Models.AssetMappings
{
    [DataContract]
    public class GetAssetMappingResponse
    {
        [DataMember(Order =1)]
        public AssetMapping AssetMapping { get; set; }

        [DataMember(Order = 2)]
        public ErrorResponse Error { get; set; }
    }
}