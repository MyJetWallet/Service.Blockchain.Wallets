using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.Blockchain.Wallets.Postgres.Migrations
{
    public partial class WalletSignature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "blockchain-wallets",
                table: "addresses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Signature",
                schema: "blockchain-wallets",
                table: "addresses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SigningKeyId",
                schema: "blockchain-wallets",
                table: "addresses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "blockchain-wallets",
                table: "addresses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE \"blockchain-wallets\".addresses as upd "+
                                  "SET \"Tag\" = \'\' " +
                                  "WHERE upd.\"Tag\" is null");

            migrationBuilder.Sql("UPDATE \"blockchain-wallets\".addresses as upd " +
                                  "SET \"Status\" = 3 " +
                                  "WHERE upd.\"Status\" = 2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "blockchain-wallets",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "Signature",
                schema: "blockchain-wallets",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "SigningKeyId",
                schema: "blockchain-wallets",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "blockchain-wallets",
                table: "addresses");
        }
    }
}
