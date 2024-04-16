using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameStatistic_Game_Id",
                table: "GameStatistic");

            migrationBuilder.CreateIndex(
                name: "IX_GameStatistic_GameId",
                table: "GameStatistic",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameStatistic_Game_GameId",
                table: "GameStatistic",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameStatistic_Game_GameId",
                table: "GameStatistic");

            migrationBuilder.DropIndex(
                name: "IX_GameStatistic_GameId",
                table: "GameStatistic");

            migrationBuilder.AddForeignKey(
                name: "FK_GameStatistic_Game_Id",
                table: "GameStatistic",
                column: "Id",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
