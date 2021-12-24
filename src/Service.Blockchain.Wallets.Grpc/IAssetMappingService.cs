using System.ServiceModel;
using System.Threading.Tasks;
using Service.Blockchain.Wallets.Grpc.Models.AssetMappings;

namespace Service.Blockchain.Wallets.Grpc
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