using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.AssetsDictionary.Client;
using Service.Fireblocks.Api.Client;
using Service.Blockchain.Wallets.MyNoSql.Addresses;
using Service.Blockchain.Wallets.MyNoSql.AssetsMappings;
using Service.Circle.Signer.Client;
using Service.Circle.Wallets.Client;
using MyJetWallet.Circle.Settings.Ioc;
using Service.Blockchain.Wallets.Job;
using Service.Blockchain.Wallets.Job.Jobs;

namespace Service.Blockchain.Wallets.Job.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var myNoSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);
            builder.RegisterFireblocksApiClient(Program.Settings.FireblocksApiUrl);
            builder.RegisterMyNoSqlReader<AssetMappingNoSql>(myNoSqlClient, AssetMappingNoSql.TableName);
            //builder.RegisterMyNoSqlReader<VaultAddressNoSql>(myNoSqlClient, VaultAddressNoSql.TableName);
            builder.RegisterAssetsDictionaryClients(myNoSqlClient);
            builder.RegisterAssetPaymentSettingsClients(myNoSqlClient);

            builder.RegisterCirclePaymentsClient(Program.Settings.CircleSignerGrpcServiceUrl);
            builder.RegisterCircleWalletsClient(myNoSqlClient, Program.Settings.CircleWalletsGrpcServiceUrl);
            builder.RegisterCircleSettingsReader(myNoSqlClient);
            builder.RegisterCircleSettingsWriter(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl));
            builder.RegisterCircleDepositAddressClient(Program.Settings.CircleSignerGrpcServiceUrl);

            builder
                .RegisterType<DepositAddressesGenerationJob>()
                .AutoActivate()
                .SingleInstance()
                .AsSelf();
        }
    }
}