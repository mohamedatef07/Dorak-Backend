using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ProviderConfiguration:IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            //PrimaryKey
            builder.HasKey(provider=>provider.ProviderId);

            //Relations
            builder.HasOne(p => p.User)
                .WithOne(u => u.Provider)
                .HasForeignKey<Provider>(p => p.ProviderId);

            builder.HasMany(p=>p.Certifications)
                .WithOne(c=>c.Provider)
                .HasForeignKey(c=>c.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ProviderAssignments)
                .WithOne(pa => pa.Provider)
                .HasForeignKey(pa => pa.ProviderId);

            builder.HasMany(p => p.ProviderServices)
                .WithOne(ps => ps.Provider)
                .HasForeignKey(ps => ps.ProviderId);

            builder.HasMany(p => p.Appointments)
                .WithOne(app => app.Provider)
                .HasForeignKey(app => app.ProviderId);

            //Properties
            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.LastName)
                .HasMaxLength(50)
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.Specialization)
                .HasMaxLength(50)
                .HasColumnName("Specialization")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.Bio)
                .HasMaxLength(1000)
                .HasColumnName("Description")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.ExperienceYears)
                .HasColumnType("INT")
                .IsRequired(false);

            builder.Property(p => p.LicenseNumber)
                .HasMaxLength(20)
                .HasColumnType("NVARCHAR")
                .IsRequired(true);
            
            builder.Property(p => p.Gender)
                .HasMaxLength(10)
                .HasColumnName("Gender")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.BirthDate)
                .HasColumnType("DATE")
                .IsRequired(true);

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
