using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSupermarketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AmountwPaidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$ZbfB.2Z/7ctp4y0T9Q6eh.GFuFaim.AT1tCKlo8dwvaRRqxn.oN8u");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$mHIxK7wo.oYb.BLPsHJqWOEyUmpTHiX1gvsxi8cAvt9Icp4BO1gqK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
