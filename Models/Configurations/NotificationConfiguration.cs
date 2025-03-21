using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {

            //Primary Key
            builder.HasKey(notification => notification.NotificationID);

            //Relations Many To one
            builder.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserID);

            //Properties
            builder.Property(notification => notification.Title)
                .IsRequired(true);
            builder.Property(notification => notification.Message)
                .IsRequired(true);
            builder.Property(notification => notification.Type)
                .IsRequired(true);
            builder.Property(notification => notification.IsRead)
                .IsRequired(true);
            builder.Property(notification => notification.CreatedAt)
                .IsRequired(true);
            builder.Property(notification => notification.ExpiredAt)
                .IsRequired(true);
            builder.Property(notification => notification.DeliveryMethod)
                .IsRequired(true);
        }

    }
}
