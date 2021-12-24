using MyJetWallet.Fireblocks.Domain.Models.AssetMappngs;
using System.Runtime.Serialization;

namespace Service.Blockchain.Wallets.Grpc.Models.AssetMappings
{
    [DataContract]
    public class UpsertAssetMappingRequest
    {
        [DataMember(Order = 1)]
        public AssetMapping AssetMapping { get; set; }
    }
}