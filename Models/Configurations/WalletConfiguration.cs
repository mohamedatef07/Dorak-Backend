using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Models;


namespace Models.Configurations
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
                .HasForeignKey<Wallet>(w => w.ClientId);

            //Properties
            builder.Property(wallet => wallet.Balance)
                .IsRequired(true);
        }
    }
}
