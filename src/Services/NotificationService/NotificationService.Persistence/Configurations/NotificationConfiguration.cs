using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationService.Domain.Notifications;

namespace NotificationService.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new NotificationId(value))
            .IsRequired();

        builder.Property(x => x.Recipient)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Subject)
            .HasMaxLength(500);

        builder.Property(x => x.Message)
            .IsRequired();

        builder.Property(x => x.NotificationType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.SentAt);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.TemplateId);

        // Indexes
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Recipient);
    }
}
