using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configurations
{
    public class CenterConfiguration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            //Primary Key
            builder.HasKey(center => center.CenterID);

            //Relations Many To one
            builder.HasMany(center => center.AdminCentersManagement)
                .WithOne(AdminCenterManagement => AdminCenterManagement.Center)
                .HasForeignKey(AdminCenterManagement => AdminCenterManagement.CenterID);

            builder.HasMany(center => center.CenterServices)
                .WithOne(centerService => centerService.Center)
                .HasForeignKey(centerService => centerService.CenterID);

            

            //Properties
            builder.Property(center => center.CenterName)
                .IsRequired(true);

            builder.Property(center => center.ContactNumber)
                .IsRequired(true);

            builder.Property(center => center.Address)
                .IsRequired(true);

            builder.Property(center => center.WebsiteURL)
                .IsRequired(false);

            builder.Property(center => center.MapURL)
                .IsRequired(false);

            builder.Property(center => center.Latitude)
                .IsRequired(false);

            builder.Property(center => center.Longitude)
                .IsRequired(false);


        }
    }
}
