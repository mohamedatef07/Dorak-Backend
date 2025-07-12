using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class AdminCenterManagementConfiguration : IEntityTypeConfiguration<AdminCenterManagement>
    {
        public void Configure(EntityTypeBuilder<AdminCenterManagement> builder)
        {
            builder.HasQueryFilter(adminCenterManagement => !adminCenterManagement.IsDeleted);
            //Primary Key
            builder.HasKey(adminCenterManagement => adminCenterManagement.AdminId);

            // Relations Many to One
            builder.HasOne(adminCenterManagement => adminCenterManagement.Admin)
                .WithOne(user => user.AdminCentersManagement)
                .HasForeignKey<AdminCenterManagement>(adminCenterManagement => adminCenterManagement.AdminId)
                .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(adminCenterManagement => adminCenterManagement.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
