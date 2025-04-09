using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ProviderCenterServiceConfiguration : IEntityTypeConfiguration<ProviderCenterService>
    {
        public void Configure(EntityTypeBuilder<ProviderCenterService> builder)
        {
            // Primary Key
            builder.HasKey(x => x.ProviderCenterServiceId);

            // Relations
            builder.HasMany(o => o.Appointments)
                .WithOne(s => s.ProviderCenterService)
                .HasForeignKey(s => s.ProviderCenterServiceId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
