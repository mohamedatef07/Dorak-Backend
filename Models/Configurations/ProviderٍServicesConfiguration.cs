using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configrations
{
    public class ProviderServicesConfiguration : IEntityTypeConfiguration<ProviderServices>
    {
        public void Configure(EntityTypeBuilder<ProviderService> builder)
        {
            builder.HasKey(ProviderServices => ProviderServices.ID);



			builder.HasMany(provider => provider.ProviderServices)
				WithOne(providerService => providerService.Provider)
			  .HasForeignKey(providerService => providerService.ProviderID) 

			builder.HasMany(service => service.ProviderServices)
				.WithOne(providerService => providerService.Service)  
				.HasForeignKey(providerService => providerService.ServiceID)  


		}
	}
}
