using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configrations
{
    public class ProviderServicesConfiguration : IEntityTypeConfiguration<ProviderService>
    {
        public void Configure(EntityTypeBuilder<ProviderService> builder)
        {
            builder.HasKey(ProviderServices => ProviderServices.ProviderServiceId);
		}
	}
}
