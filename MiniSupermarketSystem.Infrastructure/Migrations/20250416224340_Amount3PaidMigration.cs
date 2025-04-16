using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSupermarketSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Amount3PaidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT * FROM sys.columns 
                       WHERE object_id = OBJECT_ID('Orders') 
                       AND name = 'AmountPaid')
        BEGIN
            ALTER TABLE Orders ADD AmountPaid DECIMAL(18,2) NOT NULL DEFAULT 0;
        END
    ");
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$L9yUysb83/yJKjnXiAqWOezTFf7dDo/pnpNw0OaQw6ke2lI8eVQaC");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$sNS7fqKJbUakF.XsXNJAzucOk/D.M1Eg1vnAlKHoOdzMmBhnmHL3K");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
