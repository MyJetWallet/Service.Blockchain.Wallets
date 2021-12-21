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

        [OperationContract]
        Task<UpsertAssetMappingResponse> UpsertAssetMappingAsync(UpsertAssetMappingRequest request);

        [OperationContract]
        Task<DeleteAssetMappingResponse> DeleteAssetMappingAsync(DeleteAssetMappingRequest request);
    }
}