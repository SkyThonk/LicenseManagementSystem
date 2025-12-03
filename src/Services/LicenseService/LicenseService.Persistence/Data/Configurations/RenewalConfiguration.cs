using LicenseService.Domain.Renewals;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseService.Persistence.Data.Configurations;

internal class RenewalConfiguration : IEntityTypeConfiguration<Renewal>
{
    public void Configure(EntityTypeBuilder<Renewal> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).HasConversion(
            id => id.Value,
            value => new RenewalId(value)
        );

        builder.Property(r => r.LicenseId).HasConversion(
            id => id.Value,
            value => new LicenseId(value)
        ).IsRequired();

        builder.HasIndex(r => r.LicenseId);

        builder.Property(r => r.RenewalDate)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<RenewalStatus>(s)
            )
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(r => r.Status);

        builder.Property(r => r.ProcessedAt);

        // Relationship with License
        builder.HasOne(r => r.License)
            .WithMany()
            .HasForeignKey(r => r.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("renewals");
    }
}
