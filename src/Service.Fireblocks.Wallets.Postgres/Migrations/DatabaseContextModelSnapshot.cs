﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Service.Fireblocks.Wallets.Postgres;

#nullable disable

namespace Service.Fireblocks.Wallets.Postgres.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("fireblocks-wallets")
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MyJetWallet.Fireblocks.Domain.Models.VaultAccounts.VaultAccount", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("AutoFuel")
                        .HasColumnType("boolean");

                    b.Property<string>("CustomerRefId")
                        .HasColumnType("text");

                    b.Property<bool>("HiddenOnUI")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("vault_accounts", "fireblocks-wallets");
                });

            modelBuilder.Entity("MyJetWallet.Fireblocks.Domain.Models.VaultAssets.VaultAsset", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<decimal>("Available")
                        .HasColumnType("numeric");

                    b.Property<string>("BlockHash")
                        .HasColumnType("text");

                    b.Property<string>("BlockHeight")
                        .HasColumnType("text");

                    b.Property<decimal>("Frozen")
                        .HasColumnType("numeric");

                    b.Property<decimal>("LockedAmount")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Pending")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Staked")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Total")
                        .HasColumnType("numeric");

                    b.Property<string>("VaultAccountId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("VaultAccountId");

                    b.ToTable("VaultAsset", "fireblocks-wallets");
                });

            modelBuilder.Entity("Service.Fireblocks.Wallets.Postgres.Entities.UserAddressEntity", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("FireblocksAssetId")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("AssetId")
                        .HasColumnType("text");

                    b.Property<decimal>("Bip44AddressIndex")
                        .HasColumnType("numeric");

                    b.Property<string>("EnterpriseAddress")
                        .HasColumnType("text");

                    b.Property<string>("FireblocksVaultAccountId")
                        .HasColumnType("text");

                    b.Property<string>("LegacyAddress")
                        .HasColumnType("text");

                    b.Property<string>("NetworkId")
                        .HasColumnType("text");

                    b.Property<string>("Tag")
                        .HasColumnType("text");

                    b.HasKey("UserId", "FireblocksAssetId");

                    b.ToTable("addresses", "fireblocks-wallets");
                });

            modelBuilder.Entity("MyJetWallet.Fireblocks.Domain.Models.VaultAssets.VaultAsset", b =>
                {
                    b.HasOne("MyJetWallet.Fireblocks.Domain.Models.VaultAccounts.VaultAccount", null)
                        .WithMany("VaultAssets")
                        .HasForeignKey("VaultAccountId");
                });

            modelBuilder.Entity("MyJetWallet.Fireblocks.Domain.Models.VaultAccounts.VaultAccount", b =>
                {
                    b.Navigation("VaultAssets");
                });
#pragma warning restore 612, 618
        }
    }
}