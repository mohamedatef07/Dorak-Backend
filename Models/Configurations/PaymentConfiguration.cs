using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            //Primary Key
            builder.HasKey(payment => payment.PaymentID);

            //Relations Many To one
            builder.HasOne(payment => payment.Client)
                .WithMany(user => user.Payments)
                .HasForeignKey(payment => payment.ClientID);

            builder.HasMany(payment=> payment.Notifications)
                .WithOne(notification=>notification.Payment)
                .HasForeignKey(notification=>notification.PaymentID);

            builder.HasOne(payment => payment.Appointment)
                .WithOne(appointment => appointment.Payment)
                .HasForeignKey<Payment>(p => p.AppointmentID);

            //Properties
            builder.Property(payment => payment.Amount)
                .IsRequired(true);
            builder.Property(payment => payment.PaymentMethod)
                .IsRequired(true);
            builder.Property(payment => payment.TransactionID)
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
