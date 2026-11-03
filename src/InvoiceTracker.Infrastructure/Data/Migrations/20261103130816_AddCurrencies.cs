using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InvoiceTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Code = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    MinorUnits = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Code);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Code", "MinorUnits", "Name" },
                values: new object[,]
                {
                    { "EUR", 2, "Euro" },
                    { "GBP", 2, "Pound Sterling" },
                    { "INR", 2, "Indian Rupee" },
                    { "USD", 2, "US Dollar" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Currency",
                table: "Invoices",
                column: "Currency");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Currencies_Currency",
                table: "Invoices",
                column: "Currency",
                principalTable: "Currencies",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Currencies_Currency",
                table: "Invoices");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Currency",
                table: "Invoices");
        }
    }
}
