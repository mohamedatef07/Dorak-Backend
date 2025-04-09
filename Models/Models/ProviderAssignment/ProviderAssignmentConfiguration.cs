using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
	public class ProviderAssignmentConfiguration : IEntityTypeConfiguration<ProviderAssignment>
	{
		public void Configure(EntityTypeBuilder<ProviderAssignment> builder)
		{
			// Primary Key
			builder.HasKey(ProviderAssignment => ProviderAssignment.AssignmentId);

			// Relations
            builder.HasMany(o => o.Shifts)
                .WithOne(s => s.ProviderAssignment)
                .HasForeignKey(s => s.ProviderAssignmentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
	}
}