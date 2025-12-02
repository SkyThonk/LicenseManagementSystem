using TenantService.Domain.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TenantService.Persistence.Data.Configurations;

internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasConversion(
            id => id.Value,
            value => new TenantId(value)
        );

        builder.Property(t => t.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.AgencyCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(t => t.AgencyCode)
            .IsUnique();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Logo)
            .HasMaxLength(500);

        builder.Property(t => t.IsActive)
            .IsRequired();

        // Owned Types
        builder.OwnsOne(t => t.Address, address =>
        {
            address.Property(a => a.AddressLineOne).HasColumnName("AddressLineOne").HasMaxLength(255).IsRequired();
            address.Property(a => a.AddressLineTwo).HasColumnName("AddressLineTwo").HasMaxLength(255);
            address.Property(a => a.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            address.Property(a => a.State).HasColumnName("State").HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
            address.Property(a => a.CountryCode).HasColumnName("CountryCode").HasMaxLength(10);
        });

        builder.OwnsOne(t => t.Phone, phone =>
        {
            phone.Property(p => p.CountryCode).HasColumnName("PhoneCountryCode").HasMaxLength(5).IsRequired();
            phone.Property(p => p.Number).HasColumnName("PhoneNumber").HasMaxLength(15).IsRequired();
        });

        builder.ToTable("tenants");
    }
}

