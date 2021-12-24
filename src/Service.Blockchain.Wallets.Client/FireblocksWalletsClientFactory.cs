using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyNoSqlServer.DataReader;
using Service.Blockchain.Wallets.Grpc;

namespace Service.Blockchain.Wallets.Client
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
