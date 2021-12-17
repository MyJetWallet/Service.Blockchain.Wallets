using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Fireblocks.Wallets.Postgres.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "fireblocks-wallets");

            migrationBuilder.CreateTable(
                name: "addresses",
                schema: "fireblocks-wallets",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FireblocksAssetId = table.Column<string>(type: "text", nullable: false),
                    FireblocksVaultAccountId = table.Column<string>(type: "text", nullable: true),
                    AssetId = table.Column<string>(type: "text", nullable: true),
                    NetworkId = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Tag = table.Column<string>(type: "text", nullable: true),
                    LegacyAddress = table.Column<string>(type: "text", nullable: true),
                    Bip44AddressIndex = table.Column<decimal>(type: "numeric", nullable: false),
                    EnterpriseAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => new { x.UserId, x.FireblocksAssetId });
                });

            migrationBuilder.CreateTable(
                name: "vault_accounts",
                schema: "fireblocks-wallets",
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
                schema: "fireblocks-wallets",
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
                        principalSchema: "fireblocks-wallets",
                        principalTable: "vault_accounts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VaultAsset_VaultAccountId",
                schema: "fireblocks-wallets",
                table: "VaultAsset",
                column: "VaultAccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addresses",
                schema: "fireblocks-wallets");

            migrationBuilder.DropTable(
                name: "VaultAsset",
                schema: "fireblocks-wallets");

            migrationBuilder.DropTable(
                name: "vault_accounts",
                schema: "fireblocks-wallets");
        }
    }
}
