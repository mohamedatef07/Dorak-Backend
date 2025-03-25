using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class CenterConfiguration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            //Primary Key
            builder.HasKey(center => center.CenterId);

            //Relations One to Many
            builder.HasMany(center => center.AdminCentersManagement)
                .WithOne(AdminCenterManagement => AdminCenterManagement.Center)
                .HasForeignKey(AdminCenterManagement => AdminCenterManagement.CenterId);

            builder.HasMany(center => center.CenterServices)
                .WithOne(centerService => centerService.Center)
                .HasForeignKey(centerService => centerService.CenterId);

            builder.HasMany(center => center.ProviderAssignments)
                .WithOne(provderAssignment => provderAssignment.Center)
                .HasForeignKey(provderAssignment => provderAssignment.CenterId);


            builder.HasMany(center => center.ProviderAssignments)
                .WithOne(provderAssignment => provderAssignment.Center)
                .HasForeignKey(provderAssignment => provderAssignment.CenterID);

            //Properties
            builder.Property(center => center.CenterName)
                .HasMaxLength(50)
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(center => center.ContactNumber)
                .HasMaxLength(25)
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(center => center.WebsiteURL)
                .IsRequired(false);

            builder.Property(center => center.MapURL)
                .IsRequired(false);

<<<<<<< HEAD:Models/Models/Center/CenterConfiguration.cs
            builder.Property(center => center.IsDeleted)
                .HasDefaultValue(false);
=======
            builder.Property(center => center.Latitude)
                .IsRequired(false);

            builder.Property(center => center.Longitude)
                .IsRequired(false);
>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Configurations/CenterConfiguration.cs
        }
    }
}
