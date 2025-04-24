using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Dorak.Models
{
    public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
    {
        public void Configure(EntityTypeBuilder<Operator> builder)
        {
            //Primary Key
            builder.HasKey(oper => oper.OperatorId);

            //Relations 
            builder.HasOne(o => o.User)
                .WithOne(u => u.Operator)
                .HasForeignKey<Operator>(o => o.OperatorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(o => o.Shifts)
                .WithOne(s => s.Operator)
                .HasForeignKey(s => s.OperatorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(o => o.LiveQueues)
                .WithOne(lq => lq.Operator)
                .HasForeignKey(lq => lq.OperatorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(o => o.Appointments)
                .WithOne(a => a.Operator)
                .HasForeignKey(a => a.OperatorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(o => o.TemporaryClients)
                .WithOne(tc => tc.Operator)
                .HasForeignKey(tc => tc.OperatorId)
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

                .IsRequired(true);

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);
        }
    }

}
