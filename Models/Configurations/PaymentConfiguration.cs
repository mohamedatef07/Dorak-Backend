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
            builder.HasOne(p => p.Client)
                .WithMany(c => c.Payments)
                .HasForeignKey(p => p.ClientID);

            builder.HasOne(p => p.Appointment)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.AppointmentID);

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
