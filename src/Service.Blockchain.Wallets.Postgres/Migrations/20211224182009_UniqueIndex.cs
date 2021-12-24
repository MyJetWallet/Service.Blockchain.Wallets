using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Blockchain.Wallets.Postgres.Migrations
{
    public partial class UniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addresses_WalletId_AssetSymbol_AssetNetwork",
                schema: "blockchain-wallets",
                table: "addresses");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_WalletId_AssetSymbol_AssetNetwork",
                schema: "blockchain-wallets",
                table: "addresses",
                columns: new[] { "WalletId", "AssetSymbol", "AssetNetwork" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addresses_WalletId_AssetSymbol_AssetNetwork",
                schema: "blockchain-wallets",
                table: "addresses");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_WalletId_AssetSymbol_AssetNetwork",
                schema: "blockchain-wallets",
                table: "addresses",
                columns: new[] { "WalletId", "AssetSymbol", "AssetNetwork" },
                unique: true,
                filter: "WalletId IS NOT NULL");
        }
    }
}
