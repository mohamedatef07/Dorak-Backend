using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models.ProviderCenterService
{
    public class ProviderCenterServiceConfiguration : IEntityTypeConfiguration<ProviderCenterService>
    {
        public void Configure(EntityTypeBuilder<ProviderCenterService> builder)
        {
            
        }
    }
}
