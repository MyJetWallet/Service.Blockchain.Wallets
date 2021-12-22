using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Fireblocks.Wallets.MyNoSql.Addresses
{
    public class VaultAddressNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-fireblocks-wallets-vaultaddresses";
        public static string GeneratePartitionKey(string userId) => $"{userId}";
        public static string GenerateRowKey(string assetId, string networkId) =>
            $"{assetId}_{networkId}";

        public string UserId { get; set; }

        public string AssetId { get; set; }

        public string AssetNetworkId { get; set; }

        public VaultAddress Address { get; set; }

        public static VaultAddressNoSql Create(string userId, string assetId, string networkId, VaultAddress vaultAddress)
        {
            return new VaultAddressNoSql()
            {
                PartitionKey = GeneratePartitionKey(userId),
                RowKey = GenerateRowKey(assetId, networkId),
                AssetId = assetId,
                AssetNetworkId = networkId,
                UserId = userId,
                Address = vaultAddress
            };
        }

    }
}
