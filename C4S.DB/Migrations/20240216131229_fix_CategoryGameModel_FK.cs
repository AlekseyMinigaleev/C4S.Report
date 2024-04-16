using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class fix_CategoryGameModel_FK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGame_Category_GameId",
                table: "CategoryGame");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGame_CategoryId",
                table: "CategoryGame",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGame_Category_CategoryId",
                table: "CategoryGame",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGame_Category_CategoryId",
                table: "CategoryGame");

            migrationBuilder.DropIndex(
                name: "IX_CategoryGame_CategoryId",
                table: "CategoryGame");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGame_Category_GameId",
                table: "CategoryGame",
                column: "GameId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
