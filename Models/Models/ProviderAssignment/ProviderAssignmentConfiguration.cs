using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
	public class ProviderAssignmentConfiguration : IEntityTypeConfiguration<ProviderAssignment>
	{
		public void Configure(EntityTypeBuilder<ProviderAssignment> builder)
		{

			builder.HasKey(ProviderAssignment => ProviderAssignment.AssignmentId);
        }
	}
}