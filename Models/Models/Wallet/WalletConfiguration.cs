using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Dorak.Models
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            //Primary Key
            builder.HasKey(wallet => wallet.WalletId);

            //Relations One To one
            builder.HasOne(w => w.Client)
                .WithOne(c => c.Wallet)
                .HasForeignKey<Wallet>(w => w.ClientId)
                .OnDelete(DeleteBehavior.NoAction);

            //Properties
            builder.Property(wallet => wallet.Balance)
                .IsRequired(true);

        }
    }
}
