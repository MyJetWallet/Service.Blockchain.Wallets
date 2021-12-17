using Autofac;
using Service.Fireblocks.Wallets.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.Fireblocks.Wallets.Client
{
    public static class AutofacHelper
    {
        public static void RegisterFireblocksWalletsClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new FireblocksWalletsClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetWalletService()).As<IWalletService>().SingleInstance();

            builder.RegisterInstance(factory.GetAssetMappingService()).As<IAssetMappingService>().SingleInstance();
        }
    }
}
