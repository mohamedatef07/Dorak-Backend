using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class WalletRepository : BaseRepository<Wallet>
    {
        public DorakContext context;
        public WalletRepository(DorakContext _context) : base(_context)
        { 
            context = _context;
        }

        public async Task<Wallet> GetWalletByUserId(string walletId)
        {
            return await base.table.FirstOrDefaultAsync(w=>w.UserId == walletId);
        }

        public async Task<bool> IncreaseBalance(string walletId, decimal amount)
        {
            var wallet = await context.Wallets.FindAsync(walletId);

            if (wallet == null) return false;

            wallet.Balance += amount;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DecreaseBalance(string walletId,decimal amount)
        {
            var wallet = await context.Wallets.FindAsync(walletId);

            if (wallet == null || wallet.Balance<amount) return false;

            wallet.Balance -= amount;
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetBalance(string walletId)
        {
            var wallet = await context.Wallets.FindAsync(walletId);

            return wallet?.Balance ?? 0;
        }
    }
}
