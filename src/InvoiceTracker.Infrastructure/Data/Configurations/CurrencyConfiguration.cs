using InvoiceTracker.Domain.Currencies;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceTracker.Infrastructure.Data.Configurations;

public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.HasKey(c => c.Code);

        builder.Property(c => c.Code)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false);
        builder.Property(c => c.Name)
            .HasMaxLength(Currency.NameMaxLength)
            .IsRequired();

        builder.HasData(
            Currency.Create("EUR", 2, "Euro"),
            Currency.Create("GBP", 2, "Pound Sterling"),
            Currency.Create("INR", 2, "Indian Rupee"),
            Currency.Create("USD", 2, "US Dollar"));
    }
}
