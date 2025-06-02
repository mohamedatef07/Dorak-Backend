using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ClientWalletAndProfileDTO
    {
        
            public string ID { get; set; }
            public string Name { get; set; }

            public string? Image { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public decimal Balance { get; set; }


    }
}
