using MyJetWallet.Fireblocks.Domain.Models.AssetMappngs;
using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.AssetMappings
{
    [DataContract]
    public class DeleteAssetMappingResponse
    {
        [DataMember(Order = 1)]
        public ErrorResponse Error { get; set; }
    }
}