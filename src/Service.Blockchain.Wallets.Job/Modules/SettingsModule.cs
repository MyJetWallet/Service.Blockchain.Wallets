using Autofac;
using Service.Blockchain.Wallets.Job;

namespace Service.Blockchain.Wallets.Job.Modules
{
    public class SettingsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(Program.Settings).AsSelf().SingleInstance();
        }
    }
}