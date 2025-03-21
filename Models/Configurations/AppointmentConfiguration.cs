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
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            //Primary Key
            builder.HasKey(Appointment => Appointment.AppointmentId);


            //One-to-Many Relations 

            //builder.HasMany(Appointment => Appointment.Payments)
            //       .WithOne(Payment => Payment.Appointment)
            //       .HasForeignKey(Payment => Payment.AppointmentId);


            //builder.HasMany(Appointment => Appointment.Notifcations)
            //       .WithOne(Notifcation => Notifcation.Appointment)
            //       .HasForeignKey(Notifcation => Notifcation.AppointmentId);


            //One-to-One Relations 

            builder.HasOne(Appointment => Appointment.Service)
                   .WithOne(Service => Service.Appointment)
                   .HasForeignKey<Appointment>(Appointment => Appointment.ServiceId);

        }
    }
}
