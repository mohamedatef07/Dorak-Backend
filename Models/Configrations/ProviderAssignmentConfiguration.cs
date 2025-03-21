using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configrations
{
	public class ProviderAssignmentConfiguration : IEntityTypeConfiguration<ProviderAssignment>
	{
		public void Configure(EntityTypeBuilder<ProviderAssignment> builder)
		{

			builder.HasKey(ProviderAssignment => ProviderAssignment.AssignmentID);

        


            builder.HasMany(provider => provider.ProviderAssignments)
              WithOne(providerAssignment => providerAssignment.Provider)  
             .HasForeignKey(providerAssignment => providerAssignment.ProviderID)


            builder..HasMany(Center => center.ProviderAssignments)
              .WithOne(providerAssignment => providerAssignment.Center)  
             .HasForeignKey(providerAssignment => providerAssignment.CenterID)  








        }
	}
}