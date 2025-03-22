using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Configurations
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
                   .HasForeignKey(Appointment => Appointment.ShiftId);
        }
    }
}
