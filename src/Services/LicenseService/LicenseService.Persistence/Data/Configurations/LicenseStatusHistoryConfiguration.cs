using LicenseService.Domain.LicenseStatusHistory;
using LicenseService.Domain.Licenses;
using LicenseService.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseService.Persistence.Data.Configurations;

internal class LicenseStatusHistoryConfiguration : IEntityTypeConfiguration<LicenseStatusHistoryEntry>
{
    public void Configure(EntityTypeBuilder<LicenseStatusHistoryEntry> builder)
    {
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id).HasConversion(
            id => id.Value,
            value => new LicenseStatusHistoryId(value)
        );

        builder.Property(h => h.TenantId)
            .IsRequired();

        builder.HasIndex(h => h.TenantId);

        builder.Property(h => h.LicenseId).HasConversion(
            id => id.Value,
            value => new LicenseId(value)
        ).IsRequired();

        builder.HasIndex(h => h.LicenseId);

        builder.Property(h => h.OldStatus)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<LicenseStatus>(s)
            )
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(h => h.NewStatus)
            .HasConversion(
                s => s.ToString(),
                s => Enum.Parse<LicenseStatus>(s)
            )
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(h => h.ChangedBy)
            .IsRequired();

        builder.HasIndex(h => h.ChangedBy);

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        builder.HasIndex(h => h.ChangedAt);

        builder.Property(h => h.Remarks)
            .HasMaxLength(1000);

        // Relationship with License
        builder.HasOne(h => h.License)
            .WithMany()
            .HasForeignKey(h => h.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("license_status_history");
    }
}
