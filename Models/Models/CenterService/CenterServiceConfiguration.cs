using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class CenterServiceConfiguration : IEntityTypeConfiguration<CenterService>
    {
        public void Configure(EntityTypeBuilder<CenterService> builder)
        {
            //Primary Key
            builder.HasKey(centerService => centerService.CenterServiceId);

            //Properties
            builder.Property(centerService => centerService.IsDeleted).HasDefaultValue(false);
        }
    }
}
