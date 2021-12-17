using System.ServiceModel;
using System.Threading.Tasks;
using Service.Fireblocks.Wallets.Grpc.Models.AssetMappings;

namespace Service.Fireblocks.Wallets.Grpc
{
    [ServiceContract]
    public interface IAssetMappingService
    {
        [OperationContract]
        Task<GetAssetMappingResponse> GetAssetMappingAsync(GetAssetMappingRequest request);

        Task<UpsertAssetMappingResponse> UpsertAssetMappingAsync(UpsertAssetMappingRequest request);
    }
}