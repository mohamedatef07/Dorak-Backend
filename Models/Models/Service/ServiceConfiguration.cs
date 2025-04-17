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

            builder.HasMany(service => service.ProviderCenterServices)
                .WithOne(providerCenterService => providerCenterService.Service)
                .HasForeignKey(providerCenterService => providerCenterService.ServiceId)
                 .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(service => service.ServiceName).IsRequired(true);

            builder.Property(service => service.Description).IsRequired(true);

            builder.Property(service => service.BasePrice).IsRequired(true);

            builder.Property(service => service.IsDeleted).HasDefaultValue(false);

        }
    }
}
