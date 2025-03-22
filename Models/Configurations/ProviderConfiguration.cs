using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;

namespace Models.Configrations
{
    public class ProviderConfiguration:IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            //PrimaryKey
            builder.HasKey(provider=>provider.UserID);

            //Relations
            builder.HasOne(p => p.User)
                .WithOne(u => u.Provider)
                .HasForeignKey<Provider>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p=>p.Certifications)
                .WithOne(c=>c.Provider)
                .HasForeignKey(c=>c.ProviderID)
                .OnDelete(DeleteBehavior.Cascade);



            

            //Properties 
            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .HasColumnName("First Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.FirstName)
                .HasMaxLength(50)
                .HasColumnName("Last Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.Specialization)
                .HasMaxLength(50)
                .HasColumnName("Last Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.Description)
                .HasMaxLength(500)
                .HasColumnName("Description")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(p => p.ExperienceYears)
                .HasMaxLength(500)
                .HasColumnName("Experience Years")
                .HasColumnType("INT")
                .IsRequired(false);

            builder.Property(p => p.LicenseNumber)
                .HasMaxLength(20)
                .HasColumnName("License Number")
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
        }
    }
}
