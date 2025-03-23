using System;

namespace Models.Models
{
    public class Wallet
    {
        public int WalletId { get; set; }
        public decimal Balance { get; set; }
        public string ClientId { get; set; }
        public virtual User Client { get; set; }
    }
}
