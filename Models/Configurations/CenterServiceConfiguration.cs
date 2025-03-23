using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configurations
{
    public class CenterServiceConfiguration : IEntityTypeConfiguration<CenterService>
    {
        public void Configure(EntityTypeBuilder<CenterService> builder)
        {
            //Primary Key
            builder.HasKey(centerService => centerService.CenterServiceId);
        }
    }
}
