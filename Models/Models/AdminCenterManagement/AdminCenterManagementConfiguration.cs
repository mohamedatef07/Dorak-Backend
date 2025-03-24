using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class AdminCenterManagementConfiguration : IEntityTypeConfiguration<AdminCenterManagement>
    {
        public void Configure(EntityTypeBuilder<AdminCenterManagement> builder)
        {
            //Primary Key
            builder.HasKey(adminCenterManagement => adminCenterManagement.AdminCenterManagementId);

            // Relations Many to One
            builder.HasOne(adminCenterManagement => adminCenterManagement.Admin)
                .WithMany(user => user.AdminCentersManagement)
                .HasForeignKey(adminCenterManagement => adminCenterManagement.AdminId);
        }
    }
}
