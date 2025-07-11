using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            //Primary Key
            builder.HasKey(r => r.ReviewId);

            builder.Property(c => c.IsDeleted)
                    .HasDefaultValue(false);
        }

    }
}
