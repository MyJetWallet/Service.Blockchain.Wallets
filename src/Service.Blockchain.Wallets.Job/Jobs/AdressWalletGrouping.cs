// ReSharper disable InconsistentLogPropertyNaming

namespace Service.Blockchain.Wallets.Job.Jobs
{
    public class AdressWalletGrouping
    {
        public string AssetSymbol { get; set; }
        public string AssetNetwork { get; set; }

        public int WalletCount { get; set; }
    }
}