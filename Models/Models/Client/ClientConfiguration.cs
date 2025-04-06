using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dorak.Models
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            //Primary Key
            builder.HasKey(client => client.ClientId);

            //Relations 
            builder.HasOne(c => c.User)
                .WithOne(u => u.Client)
                .HasForeignKey<Client>(c => c.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(c => c.FirstName)
                .HasMaxLength(50)
                .HasColumnName("First Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);


            builder.Property(c => c.LastName)
                .HasMaxLength(50)
                .HasColumnName("Last Name")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);

            builder.Property(c => c.Gender)
                .HasMaxLength(10)
                .HasColumnName("Gender")
                .HasColumnType("NVARCHAR")
                .IsRequired(true);


            builder.Property(c => c.BirthDate)
                .HasColumnType("DATE")
                .IsRequired();

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);
        }
    }
}
