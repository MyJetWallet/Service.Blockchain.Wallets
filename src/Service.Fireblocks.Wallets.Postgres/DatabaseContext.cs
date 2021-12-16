using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyJetWallet.Fireblocks.Domain.Models.VaultAccounts;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;
using Service.Fireblocks.Wallets.Postgres.Entities;

namespace Service.Fireblocks.Wallets.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "fireblocks-wallets";

        private const string VaultAccountsTableName = "vault_accounts";
        private const string VaultAssetsTableName = "vault_assets";
        private const string AddressesTableName = "addresses";

        public DbSet<VaultAccount> VaultAccounts { get; set; }

        public DbSet<UserAddressEntity> VaultAddresses { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            RegisterVaultAccounts(modelBuilder);
            RegisterVaultAddresses(modelBuilder);


            base.OnModelCreating(modelBuilder);
        }

        private static void RegisterVaultAccounts(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VaultAccount>().ToTable(VaultAccountsTableName);
            modelBuilder.Entity<VaultAccount>().HasKey(x => x.Id);
            modelBuilder.Entity<VaultAccount>().Property(e => e.Id).ValueGeneratedNever();
        }

        private static void RegisterVaultAddresses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAddressEntity>().ToTable(AddressesTableName);
            modelBuilder.Entity<UserAddressEntity>().HasKey(x => new { x.UserId, x.FireblocksAssetId });
        }
    }
}