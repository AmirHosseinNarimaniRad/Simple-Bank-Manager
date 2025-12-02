using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithStaticSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "bank_accounts",
                columns: new[] { "Id", "Balance", "CreatedAt", "Name", "UpdatedAt", "UserId" },
                values: new object[] { 1, 1000000m, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "کیف پول تست", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "transactions",
                columns: new[] { "Id", "AccountId", "Amount", "Category", "CreatedAt", "DateTime", "Description", "IncomeType", "Type", "UpdatedAt" },
                values: new object[] { 1, 1, 1000000m, "حقوق", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "تست اولیه", "حقوق", "Income", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "FirstName", "LastName", "MonthlyBudget", "PhoneNumber", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Test", "User", 5000000m, "09123456789", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "bank_accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
