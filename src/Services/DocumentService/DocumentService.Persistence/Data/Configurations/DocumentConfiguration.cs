using DocumentService.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DocumentService.Persistence.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => new DocumentId(value))
            .IsRequired();

        builder.Property(d => d.LicenseId)
            .IsRequired();

        builder.Property(d => d.DocumentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.FileUrl)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(d => d.MimeType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.SizeInKb);

        builder.Property(d => d.UploadedBy)
            .IsRequired();

        builder.Property(d => d.UploadedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(d => d.LicenseId);
        builder.HasIndex(d => d.DocumentType);
        builder.HasIndex(d => d.UploadedBy);
        builder.HasIndex(d => d.UploadedAt);
    }
}
