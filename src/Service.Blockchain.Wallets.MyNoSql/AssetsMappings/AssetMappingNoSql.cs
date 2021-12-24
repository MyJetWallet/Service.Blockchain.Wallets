using MyJetWallet.Fireblocks.Domain.Models.AssetMappngs;
using MyNoSqlServer.Abstractions;

namespace Service.Blockchain.Wallets.MyNoSql.AssetsMappings
{
    public class AssetMappingNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fireblocks-wallets-assetmapping";
        public static string GeneratePartitionKey(string assetId) => $"{assetId}";
        public static string GenerateRowKey(string networkId) =>
            $"{networkId}";

        public AssetMapping AssetMapping { get; set; }

        public static AssetMappingNoSql Create(AssetMapping assetMapping) =>
            new()
            {
                PartitionKey = GeneratePartitionKey(assetMapping.AssetId),
                RowKey = GenerateRowKey(assetMapping.NetworkId),
                AssetMapping = assetMapping
            };
    }
}