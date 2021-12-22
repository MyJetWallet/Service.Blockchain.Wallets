using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyNoSqlServer.DataReader;
using Service.Fireblocks.Wallets.Grpc;
using Service.Fireblocks.Wallets.MyNoSql.Addresses;

// ReSharper disable UnusedMember.Global

namespace Service.Fireblocks.Wallets.Client
{
    public static class AutofacHelper
    {
        public static void RegisterFireblocksWalletsClient(this ContainerBuilder builder, string grpcServiceUrl, MyNoSqlTcpClient tcpClient)
        {
            var factory = new FireblocksWalletsClientFactory(grpcServiceUrl);

            var reader = builder.RegisterMyNoSqlReader<VaultAddressNoSql>(tcpClient, VaultAddressNoSql.TableName);
            var walletService = factory.GetWalletService();
            var cacheDecorator = new WalletServiceCacheDecorator(walletService, reader);

            builder.RegisterInstance(cacheDecorator).As<IWalletService>().SingleInstance();

            builder.RegisterInstance(factory.GetAssetMappingService()).As<IAssetMappingService>().SingleInstance();
        }
    }
}
