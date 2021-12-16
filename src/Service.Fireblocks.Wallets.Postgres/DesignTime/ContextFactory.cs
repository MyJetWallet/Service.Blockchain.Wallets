using MyJetWallet.Sdk.Postgres;
using Service.Fireblocks.Wallets.Postgres;

namespace Service.Fireblocks.Wallets.Postgres.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<DatabaseContext>
    {
        public ContextFactory() : base(options => new DatabaseContext(options))
        {

        }
    }
}