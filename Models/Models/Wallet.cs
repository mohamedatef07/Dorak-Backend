using System;

namespace Models.Models
{
    public class Wallet
    {
        public int WalletID { get; set; }
        public int ClientID { get; set; }
        public decimal Balance { get; set; }
        
        public Client Client { get; set; }
    }
}
