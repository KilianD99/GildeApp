using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GildeApp.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ClaimExpiresAt",
                table: "Matches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimedBy",
                table: "Matches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555554"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Matches",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555556"),
                columns: new[] { "ClaimExpiresAt", "ClaimedBy" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimExpiresAt",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ClaimedBy",
                table: "Matches");
        }
    }
}
