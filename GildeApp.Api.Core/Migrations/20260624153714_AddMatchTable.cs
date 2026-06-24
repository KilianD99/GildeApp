using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GildeApp.Api.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_Players_FirstPlayerId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Players_SecondPlayerId",
                table: "Match");

            migrationBuilder.DropForeignKey(
                name: "FK_Match_Tourneys_TourneyId",
                table: "Match");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Match",
                table: "Match");

            migrationBuilder.RenameTable(
                name: "Match",
                newName: "Matches");

            migrationBuilder.RenameIndex(
                name: "IX_Match_TourneyId",
                table: "Matches",
                newName: "IX_Matches_TourneyId");

            migrationBuilder.RenameIndex(
                name: "IX_Match_SecondPlayerId",
                table: "Matches",
                newName: "IX_Matches_SecondPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Match_FirstPlayerId",
                table: "Matches",
                newName: "IX_Matches_FirstPlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matches",
                table: "Matches",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_FirstPlayerId",
                table: "Matches",
                column: "FirstPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Players_SecondPlayerId",
                table: "Matches",
                column: "SecondPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Tourneys_TourneyId",
                table: "Matches",
                column: "TourneyId",
                principalTable: "Tourneys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_FirstPlayerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Players_SecondPlayerId",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Tourneys_TourneyId",
                table: "Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matches",
                table: "Matches");

            migrationBuilder.RenameTable(
                name: "Matches",
                newName: "Match");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_TourneyId",
                table: "Match",
                newName: "IX_Match_TourneyId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_SecondPlayerId",
                table: "Match",
                newName: "IX_Match_SecondPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_Matches_FirstPlayerId",
                table: "Match",
                newName: "IX_Match_FirstPlayerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Match",
                table: "Match",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Players_FirstPlayerId",
                table: "Match",
                column: "FirstPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Players_SecondPlayerId",
                table: "Match",
                column: "SecondPlayerId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_Tourneys_TourneyId",
                table: "Match",
                column: "TourneyId",
                principalTable: "Tourneys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
