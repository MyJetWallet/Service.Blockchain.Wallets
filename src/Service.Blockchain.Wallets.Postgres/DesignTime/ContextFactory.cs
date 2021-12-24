using MyJetWallet.Sdk.Postgres;
using Service.Blockchain.Wallets.Postgres;

namespace Service.Blockchain.Wallets.Postgres.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
    {
        public ContextFactory() : base(options => new DatabaseContext(options))
        {

        }
    }
}