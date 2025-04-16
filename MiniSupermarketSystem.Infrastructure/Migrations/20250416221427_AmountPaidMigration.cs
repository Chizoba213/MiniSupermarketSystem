using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSupermarketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AmountPaidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$JO1VGgJ9yw./NtRxikUcJeOGLkHG4HhM2zAGCxtkVTrTNyyR5nxPi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$moqej2.H77nK5HZlXQQBf.EX8yfW7/is25Xx9zKNtPLdETpYZUsne");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$SLT2utHVANCSEZGPIvKJo.xbQM9zVFBqfbQo4MDH1I/OTwtwFG2ji");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$1.6Js82TtpQ3ecYJhDCGvumeQTpasBciUUBxhvkmOPtCrhEJKUVhm");
        }
    }
}
