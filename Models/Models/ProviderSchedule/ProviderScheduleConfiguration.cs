using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ProviderScheduleConfiguration : IEntityTypeConfiguration<ProviderSchedule>
    {
        public void Configure(EntityTypeBuilder<ProviderSchedule> builder)
        {
            // Primary Key
            builder.HasKey(ps => ps.ProviderScheduleId);



            // Properties
            builder.Property(ps => ps.StartDate).IsRequired();
            builder.Property(ps => ps.EndDate).IsRequired();
            builder.Property(ps => ps.StartTime).IsRequired();
            builder.Property(ps => ps.EndTime).IsRequired();
            builder.Property(ps => ps.MaxPatientsPerDay).IsRequired();
        }
    }
}