using InvoiceTracker.Infrastructure.Invoices;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceTracker.Infrastructure.Data.Configurations;

public sealed class InvoiceNumberSeriesConfiguration : IEntityTypeConfiguration<InvoiceNumberSeries>
{
    public void Configure(EntityTypeBuilder<InvoiceNumberSeries> builder)
    {
        builder.HasKey(ins => ins.Year);

        builder.Property(ins => ins.Year).ValueGeneratedNever();

        builder.ToTable("InvoiceNumberSeries");
    }
}
