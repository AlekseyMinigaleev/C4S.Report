using C4S.DB.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#nullable disable

namespace C4S.DB.Migrations
{
    /// <inheritdoc />
    public partial class delete_authentification_fields_from_User_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                .AddDbContext<ReportDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("ReportDB")))
                .BuildServiceProvider();

            using var dbContext = serviceProvider.GetRequiredService<ReportDbContext>();

            var connectionString = dbContext.Database.GetConnectionString();


            using var connection = new SqlConnection(connectionString);
            var query = "SELECT Password, Id FROM [UserAuthentication]";
            var command = new SqlCommand(query, connection);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var password = reader["Password"].ToString();
                var id = Guid.Parse(reader["Id"].ToString());

                var userAuthenticationModel = dbContext.UserAuthenticationModels
                    .Single(x => x.Id == id);
                userAuthenticationModel.SetPassword(password);
            }
            reader.Close();
            dbContext.SaveChanges();


            migrationBuilder.DropColumn(
                name: "Password",
                table: "User");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
