using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            //Primary Key
            builder.HasKey(Appointment => Appointment.AppointmentId);

            //One-to-Many Relations 
            builder.HasMany(Appointment => Appointment.Notifications)
                   .WithOne(Notifcation => Notifcation.Appointment)
                   .HasForeignKey(Notifcation => Notifcation.AppointmentId)
                   .OnDelete(DeleteBehavior.NoAction);

            

        }
    }
}