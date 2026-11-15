using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameInvoiceNumberSeriesLastNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastNumber",
                table: "InvoiceNumberSeries",
                newName: "LastSequence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSequence",
                table: "InvoiceNumberSeries",
                newName: "LastNumber");
        }
    }
}
