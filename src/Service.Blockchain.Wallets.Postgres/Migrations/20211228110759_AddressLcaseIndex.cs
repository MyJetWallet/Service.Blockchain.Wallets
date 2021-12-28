using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Blockchain.Wallets.Postgres.Migrations
{
    public partial class AddressLcaseIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_addresses_AddressLowerCase_Tag",
                schema: "blockchain-wallets",
                table: "addresses",
                columns: new[] { "AddressLowerCase", "Tag" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_addresses_AddressLowerCase_Tag",
                schema: "blockchain-wallets",
                table: "addresses");
        }
    }
}
