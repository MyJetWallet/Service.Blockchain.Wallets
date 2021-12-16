using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using Service.Fireblocks.Api.Client;
using Service.Fireblocks.Wallets.MyNoSql.AssetsMappings;

namespace Service.Fireblocks.Wallets.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterFireblocksApiClient(Program.Settings.FireblocksApiUrl);
            builder.RegisterMyNoSqlWriter<AssetMappingNoSql>(() => Program.Settings.MyNoSqlWriterUrl, AssetMappingNoSql.TableName);
        }
    }
}