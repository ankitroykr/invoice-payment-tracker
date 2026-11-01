using InvoiceTracker.Domain.Invoices;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceTracker.Infrastructure.Data.Configurations;

public sealed class InvoiceLineConfiguration : IEntityTypeConfiguration<InvoiceLine>
{
    public void Configure(EntityTypeBuilder<InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.HasKey(il => il.Id);

        builder.Property(il => il.Description)
            .IsRequired()
            .HasMaxLength(InvoiceLine.DescriptionMaxLength);
        builder.Property(il => il.Quantity).HasPrecision(19, 4);

        builder.ComplexProperty(il => il.UnitPrice, unitPrice =>
        {
            unitPrice.Property(up => up.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasPrecision(19, 4);

            unitPrice.Property(up => up.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsFixedLength()
                .IsUnicode(false);
        });
    }
}
