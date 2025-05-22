using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject.ClientDTO
{
    public class ReviewByClientDTO
    {
            public string ProviderId { get; set; }
            public string ProviderName { get; set; }
            public string Review { get; set; }
            public decimal Rate { get; set; }
    }
}


