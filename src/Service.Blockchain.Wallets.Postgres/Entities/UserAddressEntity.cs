using Service.Blockchain.Wallets.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Blockchain.Wallets.Postgres.Entities
{
    public class UserAddressEntity
    {
        public bool IsActive { get; set; }
        public string BrokerId { get; set; }
        public string ClientId { get; set; }
        public string WalletId { get; set; }

        public string AssetSymbol { get; set; }

        public BlockchainIntegration Integration { get; set; }

        // Assigned on address generation
        public string AddressId { get; set; }

        public string AssetNetwork { get; set; }

        public string Address { get; set; }

        public string AddressLowerCase { get; set; }

        public string Tag { get; set; }
        public string LegacyAddress { get; set; }
        public decimal Bip44AddressIndex { get; set; }
        public string EnterpriseAddress { get; set; }

        public AddressStatus Status {get;set; }
        public string FireblocksAssetId { get; set; }
        public string FireblocksVaultAccountId { get; set; }
        public string CircleWalletId { get; set; }
        public string SigningKeyId { get; set; }
        public string Signature { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
