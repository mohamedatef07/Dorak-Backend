using System.Reflection.Emit;
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




            //Relations

           



            //Property
            builder.Property(pa => pa.StartDate)
               .IsRequired()
               .HasColumnType("timestamp");

            builder.Property(pa => pa.EndDate)
                .HasColumnType("timestamp")
                .IsRequired(false); 


        }
	}
}