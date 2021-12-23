using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Fireblocks.Wallets.Settings
{
    public class SettingsModel
    {
        [YamlProperty("FireblocksWallets.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("FireblocksWallets.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("FireblocksWallets.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("FireblocksWallets.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("FireblocksWallets.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; internal set; }

        [YamlProperty("FireblocksWallets.FireblocksApiUrl")]
        public string FireblocksApiUrl { get; internal set; }

        [YamlProperty("FireblocksWallets.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; internal set; }
    }
}
