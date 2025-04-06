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
        public WalletRepository(DorakContext _context) : base(_context){ }

        public async Task<Wallet> GetWalletByUserId(string UserId)
        {
            return await this.GetByIdAsync(w=>w.ClientId == UserId);    
        }
        public async Task UpdateWalletBalance(string userId, decimal amount)
        {
            var wallet = await this.GetWalletByUserId(userId);
            if (wallet != null) 
            {
                wallet.Balance += amount;
                this.Edit(wallet);
                //await SaveChangesAsync();
            }
        }












        //public async Task<bool> IncreaseBalance(string walletId, decimal amount)
        //{
        //    var wallet = await base..FindAsync(walletId);

        //    if (wallet == null) return false;

        //    wallet.Balance += amount;
        //    await context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<bool> DecreaseBalance(string walletId,decimal amount)
        //{
        //    var wallet = await context.Wallets.FindAsync(walletId);

        //    if (wallet == null || wallet.Balance<amount) return false;

        //    wallet.Balance -= amount;
        //    await context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<decimal> GetBalance(string walletId)
        //{
        //    var wallet = await context.Wallets.FindAsync(walletId);

        //    return wallet?.Balance ?? 0;
        //}
    }
}
