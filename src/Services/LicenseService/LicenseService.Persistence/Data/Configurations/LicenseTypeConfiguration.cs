using LicenseService.Domain.LicenseTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseService.Persistence.Data.Configurations;

internal class LicenseTypeConfiguration : IEntityTypeConfiguration<LicenseType>
{
    public void Configure(EntityTypeBuilder<LicenseType> builder)
    {
        builder.HasKey(lt => lt.Id);

        builder.Property(lt => lt.Id).HasConversion(
            id => id.Value,
            value => new LicenseTypeId(value)
        );

        builder.Property(lt => lt.TenantId)
            .IsRequired();

        builder.HasIndex(lt => lt.TenantId);

        builder.Property(lt => lt.Name)
            .HasMaxLength(200)
            .IsRequired();

        // Unique constraint on TenantId + Name
        builder.HasIndex(lt => new { lt.TenantId, lt.Name })
            .IsUnique();

        builder.Property(lt => lt.Description)
            .HasMaxLength(1000);

        builder.Property(lt => lt.FeeAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.ToTable("license_types");
    }
}
