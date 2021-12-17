using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.Fireblocks.Wallets.Grpc;

namespace Service.Fireblocks.Wallets.Client
{
    [UsedImplicitly]
    public class FireblocksWalletsClientFactory: MyGrpcClientFactory
    {
        public FireblocksWalletsClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IWalletService GetWalletService() => CreateGrpcService<IWalletService>();

        public IAssetMappingService GetAssetMappingService() => CreateGrpcService<IAssetMappingService>();
    }
}
