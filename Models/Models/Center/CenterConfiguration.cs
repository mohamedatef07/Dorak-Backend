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
                .HasForeignKey(AdminCenterManagement => AdminCenterManagement.CenterId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(center => center.ProviderCenterServices)
                .WithOne(ProviderCenterService => ProviderCenterService.Center)
                .HasForeignKey(ProviderCenterService => ProviderCenterService.CenterId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(center => center.ProviderAssignments)
                .WithOne(provderAssignment => provderAssignment.Center)
                .HasForeignKey(provderAssignment => provderAssignment.CenterId)
                 .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(center => center.Operators)
                .WithOne(o => o.Center)
                .HasForeignKey(o => o.CenterId)
                .OnDelete(DeleteBehavior.NoAction);

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

            builder.Property(center => center.IsDeleted)
                .HasDefaultValue(false);
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}