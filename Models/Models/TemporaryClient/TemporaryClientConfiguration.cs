using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class TemporaryClientConfiguration : IEntityTypeConfiguration<TemporaryClient>
    {
        public void Configure(EntityTypeBuilder<TemporaryClient> builder)
        {
            //Primary Key
            builder.HasKey(tempclient => tempclient.TempClientId);

            //Relations Many To one
            builder.HasMany(tempclient => tempclient.Appointments)
                .WithOne(Appointment => Appointment.TemporaryClient)
                .HasForeignKey(Appointment => Appointment.TemporaryClientId)
                 .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(tempclient => tempclient.ContactInfo)
                .IsRequired(true);

            builder.Property(tempclient => tempclient.ContactInfo)
                .IsRequired(false);

            builder.Property(tempclient => tempclient.TempCode)
                .IsRequired(true);
        }
    }
}
