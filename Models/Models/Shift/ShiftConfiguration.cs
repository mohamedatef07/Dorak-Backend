using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Dorak.Models
{
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            //Primary Key
            builder.HasKey(Shift => Shift.ShiftId);

            //One-to-Many Relations 
            builder.HasMany(Shift => Shift.Appointments)
                   .WithOne(Appointment => Appointment.Shift)
                   .HasForeignKey(Appointment => Appointment.ShiftId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
