using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configurations
{
    class TemporaryClientConfiguration : IEntityTypeConfiguration<TemporaryClient>
    {
        public void Configure(EntityTypeBuilder<TemporaryClient> builder)
        {
            //Primary Key
            builder.HasKey(tempclient => tempclient.TempClientID);

            //Relations Many To one
            builder.HasMany(tempclient => tempclient.Appointments)
                .WithOne(Appointment => Appointment.TemporaryClient)
                .HasForeignKey(Appointment => Appointment.TemporaryClientId);

            //Properties
            builder.Property(tempclient => tempclient.ContactInfo)
                .IsRequired(true);

            builder.Property(tempclient => tempclient.ContactInfo)
                .IsRequired(false);

            builder.Property(tempclient=>tempclient.TempCode)
                .IsRequired(true);
        }
    }
}
