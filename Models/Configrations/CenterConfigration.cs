using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configrations
{
    public class CenterConfigration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            //Primary Key
            builder.HasKey(center => center.CenterID);

            //Relations Many To one
            builder.HasMany(center => center.AdminCenterManagements)
                .WithOne(AdminCenterManagement => AdminCenterManagement.Center)
                .HasForeignKey(AdminCenterManagement => AdminCenterManagement.CenterID);

            builder.HasMany(center => center.CenterServices)
                .WithOne(centerServices => centerServices.Center)
                .HasForeignKey(centerServices => centerServices.CenterID);

            //Properties
            builder.Property(center => center.CenterName)
                .IsRequired(true);

            builder.Property(center => center.CenterNumber)
                .IsRequired(true);

            builder.Property(center => center.Address)
                .IsRequired(true);


        }
    }
}
