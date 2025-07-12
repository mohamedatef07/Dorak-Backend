using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class LiveQueueConfiguration : IEntityTypeConfiguration<LiveQueue>
    {
        public void Configure(EntityTypeBuilder<LiveQueue> builder)
        {
            //primary key
            builder.HasKey(LiveQueue => LiveQueue.LiveQueueId);



            builder.HasOne(a => a.Appointment)
                .WithOne(u => u.LiveQueue)
                .HasForeignKey<LiveQueue>(c => c.AppointmentId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}