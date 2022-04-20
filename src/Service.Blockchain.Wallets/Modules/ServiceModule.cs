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
using MyJetWallet.ApiSecurityManager.Autofac;

namespace Service.Blockchain.Wallets.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterEncryptionServiceClient();
            var myNoSqlClient = builder.CreateNoSqlClient(Program.ReloadedSettings(e => e.MyNoSqlReaderHostPort));
            builder.RegisterFireblocksApiClient(Program.Settings.FireblocksApiUrl);
            builder.RegisterMyNoSqlWriter<AssetMappingNoSql>(() => Program.Settings.MyNoSqlWriterUrl, AssetMappingNoSql.TableName);
            builder.RegisterMyNoSqlWriter<VaultAddressNoSql>(() => Program.Settings.MyNoSqlWriterUrl, VaultAddressNoSql.TableName);
            builder.RegisterAssetsDictionaryClients(myNoSqlClient);
            builder.RegisterAssetPaymentSettingsClients(myNoSqlClient);

            builder.RegisterCirclePaymentsClient(Program.Settings.CircleSignerGrpcServiceUrl);
            builder.RegisterCircleWalletsClient(myNoSqlClient, Program.Settings.CircleWalletsGrpcServiceUrl);
            builder.RegisterCircleSettingsReader(myNoSqlClient);
            builder.RegisterCircleSettingsWriter(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl));
            builder.RegisterCircleDepositAddressClient(Program.Settings.CircleSignerGrpcServiceUrl);
        }
    }
}