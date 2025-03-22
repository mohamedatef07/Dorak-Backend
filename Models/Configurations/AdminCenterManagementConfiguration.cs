using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configurations
{
    public class AdminCenterManagementConfiguration : IEntityTypeConfiguration<AdminCenterManagement>
    {
        public void Configure(EntityTypeBuilder<AdminCenterManagement> builder)
        {
            //Primary Key
            builder.HasKey(adminCenterManagement => adminCenterManagement.AdminCenterManagementID);

            // Relations Many to One
            builder.HasOne(adminCenterManagement => adminCenterManagement.Admin)
                .WithMany(user => user.AdminCentersManagement)
                .HasForeignKey(adminCenterManagement => adminCenterManagement.AdminID);
        }
    }
}
