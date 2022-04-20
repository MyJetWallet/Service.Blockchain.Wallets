using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Service.Blockchain.Wallets.Postgres.Migrations
{
    public partial class SignatureSetDateUnix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var defaultVal = DateTimeOffset.UnixEpoch.ToUnixTimeMilliseconds();
            migrationBuilder.AddColumn<long>(
                name: "SignatureSetUnixTime",
                schema: "blockchain-wallets",
                table: "addresses",
                type: "bigint",
                nullable: false,
                defaultValue: defaultVal);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignatureSetUnixTime",
                schema: "blockchain-wallets",
                table: "addresses");
        }
    }
}
