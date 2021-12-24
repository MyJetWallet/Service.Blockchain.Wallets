using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using Service.Blockchain.Wallets.Job.Jobs;

namespace Service.Blockchain.Wallets.Job
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyNoSqlClientLifeTime _noSqlTcpClient;
        private readonly DepositAddressesGenerationJob _depositAddressesGenerationJob;

        public ApplicationLifetimeManager(
            IHostApplicationLifetime appLifetime, 
            ILogger<ApplicationLifetimeManager> logger,
            MyNoSqlClientLifeTime noSqlTcpClient,
            DepositAddressesGenerationJob depositAddressesGenerationJob)
            : base(appLifetime)
        {
            _logger = logger;
            _noSqlTcpClient = noSqlTcpClient;
            this._depositAddressesGenerationJob = depositAddressesGenerationJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            _noSqlTcpClient.Start();
            _depositAddressesGenerationJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called.");
            _noSqlTcpClient.Stop();
            _depositAddressesGenerationJob.Stop();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
