using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Payments;

namespace PaymentService.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value))
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.LicenseId)
            .IsRequired();

        builder.Property(x => x.ApplicantId)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.PaidAt);

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.LicenseId);
        builder.HasIndex(x => x.ApplicantId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ReferenceNumber).IsUnique().HasFilter("\"ReferenceNumber\" IS NOT NULL");
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => new { x.TenantId, x.ApplicantId });
    }
}
