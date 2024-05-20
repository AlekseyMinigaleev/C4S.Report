using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class Cteate_UserAuthentification_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Login",
                table: "User",
                newName: "Email");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthenticationId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserAuthentication",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAuthentication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAuthentication_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthentication_UserId",
                table: "UserAuthentication",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO [UserAuthentication] (Id, UserId, PasswordHash, PasswordSalt,RefreshToken)
                SELECT NEWID(), Id, CAST(REPLICATE(CHAR(0), 16) AS VARBINARY(16)), CAST(REPLICATE(CHAR(0), 16) AS VARBINARY(16)),RefreshToken
                FROM [User]
            ");

            migrationBuilder.Sql(@"
                UPDATE [User] 
                SET AuthenticationId = ua.Id
                FROM [User] u
                INNER JOIN [UserAuthentication] ua ON u.Id = ua.UserId;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAuthentication");

            migrationBuilder.DropColumn(
                name: "AuthenticationId",
                table: "User");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "User",
                newName: "Login");
        }
    }
}
