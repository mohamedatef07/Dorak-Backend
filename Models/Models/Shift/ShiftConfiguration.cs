using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            //Primary Key
            builder.HasKey(Shift => Shift.ShiftId);

            //One - to - Many Relations
            builder.HasMany(Shift => Shift.Appointments)
                   .WithOne(Appointment => Appointment.Shift)
                   .HasForeignKey(Appointment => Appointment.ShiftId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(Shift => Shift.LiveQueues)
                     .WithOne(LiveQueue => LiveQueue.Shift)
                     .HasForeignKey(LiveQueue => LiveQueue.ShiftId)
                     .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
