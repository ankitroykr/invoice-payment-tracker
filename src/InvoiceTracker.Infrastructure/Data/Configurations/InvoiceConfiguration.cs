using InvoiceTracker.Domain.Customers;
using InvoiceTracker.Domain.Invoices;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceTracker.Infrastructure.Data.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Currency)
            .HasMaxLength(3)
            .IsFixedLength()
            .IsUnicode(false);
        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsOne(i => i.Number, number =>
        {
            number.Property(n => n.Year).HasColumnName("NumberYear");
            number.Property(n => n.Sequence).HasColumnName("NumberSequence");
            number.HasIndex(n => new { n.Year, n.Sequence }).IsUnique();
        });

        builder.HasMany(i => i.Lines)
            .WithOne()
            .HasForeignKey(l => l.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
