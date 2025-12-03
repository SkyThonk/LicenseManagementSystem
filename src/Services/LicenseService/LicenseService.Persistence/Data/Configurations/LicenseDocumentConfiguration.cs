using LicenseService.Domain.LicenseDocuments;
using LicenseService.Domain.Licenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LicenseService.Persistence.Data.Configurations;

internal class LicenseDocumentConfiguration : IEntityTypeConfiguration<LicenseDocument>
{
    public void Configure(EntityTypeBuilder<LicenseDocument> builder)
    {
        builder.HasKey(ld => ld.Id);

        builder.Property(ld => ld.Id).HasConversion(
            id => id.Value,
            value => new LicenseDocumentId(value)
        );

        builder.Property(ld => ld.LicenseId).HasConversion(
            id => id.Value,
            value => new LicenseId(value)
        ).IsRequired();

        builder.HasIndex(ld => ld.LicenseId);

        builder.Property(ld => ld.DocumentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(ld => ld.FileUrl)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(ld => ld.UploadedAt)
            .IsRequired();

        // Relationship with License
        builder.HasOne(ld => ld.License)
            .WithMany()
            .HasForeignKey(ld => ld.LicenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("license_documents");
    }
}
