using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class Add_CashIncomeValueWithProgress_and_RatingValueWithProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "GameStatistic",
                newName: "RatingProgress");

            migrationBuilder.RenameColumn(
                name: "CashIncome",
                table: "GameStatistic",
                newName: "CashIncomeProgress");

            migrationBuilder.AddColumn<double>(
                name: "CashIncomeActual",
                table: "GameStatistic",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingActual",
                table: "GameStatistic",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashIncomeActual",
                table: "GameStatistic");

            migrationBuilder.DropColumn(
                name: "RatingActual",
                table: "GameStatistic");

            migrationBuilder.RenameColumn(
                name: "RatingProgress",
                table: "GameStatistic",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "CashIncomeProgress",
                table: "GameStatistic",
                newName: "CashIncome");
        }
    }
}
