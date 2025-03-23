using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configurations
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
