using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Blockchain.Wallets.Common.Settings
{
    public class SettingsModel
    {
        [YamlProperty("BlockchainWallets.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("BlockchainWallets.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("BlockchainWallets.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("BlockchainWallets.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("BlockchainWallets.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; internal set; }

        [YamlProperty("BlockchainWallets.FireblocksApiUrl")]
        public string FireblocksApiUrl { get; internal set; }

        [YamlProperty("BlockchainWallets.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; internal set; }

        [YamlProperty("BlockchainWallets.CircleSignerGrpcServiceUrl")]
        public string CircleSignerGrpcServiceUrl { get; set; }

        [YamlProperty("BlockchainWallets.CircleWalletsGrpcServiceUrl")]
        public string CircleWalletsGrpcServiceUrl { get; set; }

        [YamlProperty("BlockchainWallets.GenerateAddressesIntervalSec")]
        public string GenerateAddressesIntervalSec { get; set; }

        [YamlProperty("BlockchainWallets.PreGeneratedAddressesCount")]
        public string PreGeneratedAddressesCount { get; set; }

        [YamlProperty("BlockchainWallets.WalletSignaturePrivateApiKeyId")]
        public string SignaturePrivateApiKeyId { get; set; }
    }
}
