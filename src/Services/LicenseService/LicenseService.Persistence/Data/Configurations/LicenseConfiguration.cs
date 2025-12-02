using LicenseService.Domain.Licenses;
using LicenseService.Domain.LicenseTypes;
using LicenseService.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseService.Persistence.Data.Configurations;

internal class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).HasConversion(
            id => id.Value,
            value => new LicenseId(value)
        );

        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.HasIndex(l => l.TenantId);

        builder.Property(l => l.ApplicantId)
            .IsRequired();

        builder.HasIndex(l => l.ApplicantId);

        builder.Property(l => l.LicenseTypeId).HasConversion(
            id => id.Value,
            value => new LicenseTypeId(value)
        ).IsRequired();

        builder.Property(l => l.Status)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<LicenseStatus>(s)
            )
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(l => l.Status);

        builder.Property(l => l.SubmittedAt)
            .IsRequired();

        builder.Property(l => l.ApprovedAt);

        builder.Property(l => l.ExpiryDate);

        builder.HasIndex(l => l.ExpiryDate);

        // Relationship with LicenseType
        builder.HasOne(l => l.LicenseType)
            .WithMany()
            .HasForeignKey(l => l.LicenseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("licenses");
    }
}
