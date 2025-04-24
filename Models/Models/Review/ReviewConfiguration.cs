using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
            public void Configure(EntityTypeBuilder<Review> builder)
            {

            //Primary Key
            builder.HasKey(r => r.ReviewId);
            
            }

    }
}
