using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            //Primary Key
            builder.HasKey(notification => notification.NotificationId);

            //Relations Many To one
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(notification => notification.Title)
                .IsRequired(true);
            builder.Property(notification => notification.Message)
                .IsRequired(true);
            builder.Property(notification => notification.IsRead)
                .IsRequired(true);
            builder.Property(notification => notification.CreatedAt)
                .IsRequired(true);
            builder.Property(notification => notification.ExpiredAt)
                .IsRequired(true);
        }

    }
}
