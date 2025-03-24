using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ProviderCertificationsConfiguration : IEntityTypeConfiguration<ProviderCertification>
    {
        public void Configure(EntityTypeBuilder<ProviderCertification> builder)
        {
            //Primary Key
            builder.HasKey(providercertifications => providercertifications.ProviderCertificationsId);
        }

    }
}
