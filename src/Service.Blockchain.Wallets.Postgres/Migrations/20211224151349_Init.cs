using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Blockchain.Wallets.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "blockchain-wallets");

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "blockchain-wallets",
                columns: table => new
                {
                    AddressId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    BrokerId = table.Column<string>(type: "text", nullable: true),
                    ClientId = table.Column<string>(type: "text", nullable: true),
                    WalletId = table.Column<string>(type: "text", nullable: true),
                    AssetSymbol = table.Column<string>(type: "text", nullable: true),
                    Integration = table.Column<int>(type: "integer", nullable: false),
                    AssetNetwork = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    AddressLowerCase = table.Column<string>(type: "text", nullable: true),
                    Tag = table.Column<string>(type: "text", nullable: true),
                    LegacyAddress = table.Column<string>(type: "text", nullable: true),
                    Bip44AddressIndex = table.Column<decimal>(type: "numeric", nullable: false),
                    EnterpriseAddress = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FireblocksAssetId = table.Column<string>(type: "text", nullable: true),
                    FireblocksVaultAccountId = table.Column<string>(type: "text", nullable: true),
                    CircleWalletId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "vault_accounts",
                schema: "blockchain-wallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    HiddenOnUI = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerRefId = table.Column<string>(type: "text", nullable: true),
                    AutoFuel = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vault_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VaultAsset",
                schema: "blockchain-wallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    Available = table.Column<decimal>(type: "numeric", nullable: false),
                    Pending = table.Column<decimal>(type: "numeric", nullable: false),
                    Staked = table.Column<decimal>(type: "numeric", nullable: false),
                    Frozen = table.Column<decimal>(type: "numeric", nullable: false),
                    LockedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BlockHeight = table.Column<string>(type: "text", nullable: true),
                    BlockHash = table.Column<string>(type: "text", nullable: true),
                    VaultAccountId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultAsset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaultAsset_vault_accounts_VaultAccountId",
                        column: x => x.VaultAccountId,
                        principalSchema: "blockchain-wallets",
                        principalTable: "vault_accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_WalletId_AssetSymbol_AssetNetwork",
                schema: "blockchain-wallets",
                table: "addresses",
                columns: new[] { "WalletId", "AssetSymbol", "AssetNetwork" });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_WalletId_ClientId_BrokerId",
                schema: "blockchain-wallets",
                table: "addresses",
                columns: new[] { "WalletId", "ClientId", "BrokerId" });

            migrationBuilder.CreateIndex(
                name: "IX_VaultAsset_VaultAccountId",
                schema: "blockchain-wallets",
                table: "VaultAsset",
                column: "VaultAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses",
                schema: "blockchain-wallets");

            migrationBuilder.DropTable(
                name: "VaultAsset",
                schema: "blockchain-wallets");

            migrationBuilder.DropTable(
                name: "vault_accounts",
                schema: "blockchain-wallets");
        }
    }
}
