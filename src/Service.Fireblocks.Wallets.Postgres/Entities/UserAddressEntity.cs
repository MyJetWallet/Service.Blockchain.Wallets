using Service.Fireblocks.Wallets.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Fireblocks.Wallets.Postgres.Entities
{
    public class UserAddressEntity
    {
        public string WalletId { get; set; }

        public string FireblocksAssetId { get; set; }

        public string FireblocksVaultAccountId { get; set; }

        public string AssetId { get; set; }

        public BlockchainIntegration Integrations { get; set; }

        public string NetworkId { get; set; }

        public string Address { get; set; }

        public string AddressLowerCase { get; set; }

        public string Tag { get; set; }
        public string LegacyAddress { get; set; }
        public decimal Bip44AddressIndex { get; set; }
        public string EnterpriseAddress { get; set; }
    }
}
