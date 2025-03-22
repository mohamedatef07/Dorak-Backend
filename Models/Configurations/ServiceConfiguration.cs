using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //Primary Key
            builder.HasKey(service => service.ServiceID);

          

            

            builder.HasMany(service => service.ProviderServices)
                .WithOne(providerService => providerService.Service)
                .HasForeignKey(providerService => providerService.ProviderID);
            builder.HasMany(service => service.Appointments)
                .WithOne(appointment => appointment.Service)
                .HasForeignKey(appointment => appointment.ServiceId);

            //Properties
            builder.Property(service => service.ServiceName).IsRequired(true);

            builder.Property(service => service.Description).IsRequired(true);

            builder.Property(service => service.Priority).IsRequired(true);

            builder.Property(service => service.BasePrice).IsRequired(true);

        }
    }
}
