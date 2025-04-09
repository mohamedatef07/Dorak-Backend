//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;


//namespace Dorak.Models
//{
//    public class ProviderServicesConfiguration : IEntityTypeConfiguration<ProviderService>
//    {
//        public void Configure(EntityTypeBuilder<ProviderService> builder)
//        {
//            builder.HasKey(ProviderServices => ProviderServices.ProviderServiceId);

//            builder.HasMany(o => o.ProviderCenterServices)
//               .WithOne(s => s.ProviderService)
//               .HasForeignKey(s => s.ProviderServiceId)
//               .OnDelete(DeleteBehavior.NoAction);
//        }
//	}
//}
