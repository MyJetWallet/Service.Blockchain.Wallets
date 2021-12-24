using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Fireblocks.Domain.Models.Addresses;
using MyJetWallet.Fireblocks.Domain.Models.VaultAccounts;
using MyJetWallet.Sdk.Postgres;
using Newtonsoft.Json;
using Service.Blockchain.Wallets.Postgres.Entities;

namespace Service.Blockchain.Wallets.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        private static readonly JsonSerializerSettings JsonSerializingSettings =
            new() { NullValueHandling = NullValueHandling.Ignore };

        public const string Schema = "blockchain-wallets";

        public const string VaultAccountsTableName = "vault_accounts";
        public const string AddressesTableName = "addresses";

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
            modelBuilder.Entity<UserAddressEntity>().HasKey(x => x.AddressId);
            modelBuilder.Entity<UserAddressEntity>().HasIndex(x => new { x.WalletId, x.ClientId, x.BrokerId });
            modelBuilder.Entity<UserAddressEntity>().HasIndex(x => new { x.AssetSymbol, x.AssetNetwork });
            modelBuilder.Entity<UserAddressEntity>().HasIndex(x => new { x.WalletId, x.AssetSymbol, x.AssetNetwork })
                //.HasFilter("WalletId IS NOT NULL")
                .IsUnique();
        }
    }
}