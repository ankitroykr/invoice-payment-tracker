using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceLineTotals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPriceCurrency",
                table: "InvoiceLines");

            migrationBuilder.RenameColumn(
                name: "UnitPriceAmount",
                table: "InvoiceLines",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyMinorUnits",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "InvoiceLines",
                type: "decimal(19,4)",
                precision: 19,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            // Fills in the new columns for invoices saved before this migration.
            migrationBuilder.Sql("""
                 UPDATE i
                 SET CurrencyMinorUnits = c.MinorUnits
                 FROM Invoices AS i
                 JOIN Currencies AS c ON c.Code = i.Currency;

                 UPDATE l
                 SET Total = ROUND(l.Quantity * l.UnitPrice, i.CurrencyMinorUnits)
                 FROM InvoiceLines AS l
                 JOIN Invoices AS i ON i.Id = l.InvoiceId;
                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyMinorUnits",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "InvoiceLines");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "InvoiceLines",
                newName: "UnitPriceAmount");

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceCurrency",
                table: "InvoiceLines",
                type: "char(3)",
                unicode: false,
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            // Puts each line's currency back from its invoice.
            migrationBuilder.Sql("""
                 UPDATE l
                 SET UnitPriceCurrency = i.Currency
                 FROM InvoiceLines AS l
                 JOIN Invoices AS i ON i.Id = l.InvoiceId;
                 """);
        }
    }
}
