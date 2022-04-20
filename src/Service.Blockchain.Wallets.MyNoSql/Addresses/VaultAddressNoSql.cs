using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Blockchain.Wallets.MyNoSql.Addresses
{
    public class VaultAddressNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-blockchain-wallets-vaultaddresses";
        public static string GeneratePartitionKey(string walletId) => $"{walletId}";
        public static string GenerateRowKey(string assetSymbol, string assetNetwork) =>
            $"{assetSymbol}_{assetNetwork}";

        public string WalletId { get; set; }

        public string AssetSymbol { get; set; }

        public string AssetNetwork { get; set; }

        public VaultAddress Address { get; set; }

        public string ClientId { get; set; }
        public string BrokerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SigningKeyId { get; set; }
        public string Signature { get; set; }

        public static VaultAddressNoSql Create(
            string userId, 
            string assetSymbol, 
            string assetNetwork, 
            VaultAddress vaultAddress,
            string clientId,
            string brokerId,
            DateTime createdAt,
            string signingKeyId,
            string signature)
        {
            return new VaultAddressNoSql()
            {
                ClientId = clientId,
                BrokerId = brokerId,
                CreatedAt = createdAt,
                PartitionKey = GeneratePartitionKey(userId),
                RowKey = GenerateRowKey(assetSymbol, assetNetwork),
                AssetSymbol = assetSymbol,
                AssetNetwork = assetNetwork,
                WalletId = userId,
                Address = vaultAddress,
                SigningKeyId = signingKeyId,
                Signature = signature
            };
        }
    }
}
