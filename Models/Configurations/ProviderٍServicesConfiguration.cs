using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Models.Configurations
{
    public class ProviderServicesConfiguration : IEntityTypeConfiguration<ProviderService>
    {

        public void Configure(EntityTypeBuilder<ProviderService> builder)
        {
            //Primary Key
            builder.HasKey(ProviderServices => ProviderServices.ID);

            //Relations Many To one

;


            //Property
            builder.Property(ps => ps.CustomPrice)
                   .HasColumnType("decimal(18,2)");


        }
    }
}
