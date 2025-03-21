using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;



namespace Models.Configrations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //Primary Key
            builder.HasKey(service => service.ServiceID);

            //Relations Many To one
            builder.HasMany(service => service.CenterServices)
                .WithOne(centerService => centerService.Service)
                .HasForeignKey(centerService => centerService.ServiceID);

            //Properties
            builder.Property(service => service.ServiceName).IsRequired(true);

            builder.Property(service => service.Description).IsRequired(true);

            builder.Property(service => service.Priority).IsRequired(true);

            builder.Property(service => service.BasePrice).IsRequired(true);

        }
    }
}
