using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            //Primary Key
            builder.HasKey(payment => payment.PaymentId);

            //Relations Many To one
            builder.HasOne(payment => payment.Client)
                .WithMany(user => user.Payments)
                .HasForeignKey(payment => payment.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(payment => payment.Notifications)
                .WithOne(notification => notification.Payment)
                .HasForeignKey(notification => notification.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(payment => payment.Appointment)
                .WithOne(appointment => appointment.Payment)
                .HasForeignKey<Payment>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(payment => payment.Amount)
                .IsRequired(true);
            builder.Property(payment => payment.PaymentMethod)
                .IsRequired(true);
            builder.Property(payment => payment.TransactionId)
                .IsRequired(true);
            builder.Property(payment => payment.PaymentStatus)
                .IsRequired(true);
            builder.Property(payment => payment.RefundStatus)
                .IsRequired(true);
            builder.Property(payment => payment.TransactionDate)
                .IsRequired(true);


        }

    }
}
