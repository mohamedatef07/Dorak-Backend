using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

<<<<<<< HEAD:Models/Models/Service/ServiceConfiguration.cs
namespace Dorak.Models
=======
namespace Models.Configurations
>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Configurations/ServiceConfiguration.cs
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //Primary Key
            builder.HasKey(service => service.ServiceId);

<<<<<<< HEAD:Models/Models/Service/ServiceConfiguration.cs
            //Relations One to One
            builder.HasMany(service => service.CenterServices)
                .WithOne(centerService => centerService.Service)
                .HasForeignKey(centerService => centerService.ServiceId);

            builder.HasMany(service => service.ProviderServices)
                .WithOne(providerService => providerService.Service)
                .HasForeignKey(providerService => providerService.ServiceId);
=======
          

            

            builder.HasMany(service => service.ProviderServices)
                .WithOne(providerService => providerService.Service)
                .HasForeignKey(providerService => providerService.ProviderID);
            builder.HasMany(service => service.Appointments)
                .WithOne(appointment => appointment.Service)
                .HasForeignKey(appointment => appointment.ServiceId);
>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Configurations/ServiceConfiguration.cs

            //Properties
            builder.Property(service => service.ServiceName).IsRequired(true);

            builder.Property(service => service.Description).IsRequired(true);

            builder.Property(service => service.Priority).IsRequired(true);

            builder.Property(service => service.BasePrice).IsRequired(true);

            builder.Property(service => service.IsDeleted).HasDefaultValue(false);

        }
    }
}
