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
    public class ProviderCertificationsConfiguration : IEntityTypeConfiguration<ProviderCertifications>
    {
        public void Configure(EntityTypeBuilder<ProviderCertifications> builder)
        {
            //Primary Key
            builder.HasKey(providercertifications => providercertifications.ProviderCertificationsID);

            //Relations

            //Properties


        }

    }
}
