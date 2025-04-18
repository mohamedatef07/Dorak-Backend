using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Dorak.Models
{
    public class LiveQueueConfiguration : IEntityTypeConfiguration<LiveQueue>
    {
        public void Configure(EntityTypeBuilder<LiveQueue> builder)
        {
            //primary key
            builder.HasKey(LiveQueue => LiveQueue.LiveQueueId);


            builder.HasOne(LiveQueue => LiveQueue.Appointment)
                   .WithOne(Appointment => Appointment.LiveQueue)
                   .HasForeignKey<Appointment>(Appointment => Appointment.LiveQueueId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Appointment)
                .WithOne(u => u.LiveQueue)
                .HasForeignKey<LiveQueue>(c => c.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(lq => lq.Shift)
                .WithOne(s => s.LiveQueue)
                .HasForeignKey<LiveQueue>(lq => lq.ShiftId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
