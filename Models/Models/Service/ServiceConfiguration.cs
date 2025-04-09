using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //Primary Key
            builder.HasKey(service => service.ServiceId);

            //Relations One to One
            builder.HasMany(service => service.ProviderServices)
                .WithOne(providerService => providerService.Service)
                .HasForeignKey(providerService => providerService.ServiceId)
                 .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(service => service.ServiceName).IsRequired(true);

            builder.Property(service => service.Description).IsRequired(true);

            builder.Property(service => service.BasePrice).IsRequired(true);

            builder.Property(service => service.IsDeleted).HasDefaultValue(false);

        }
    }
}
