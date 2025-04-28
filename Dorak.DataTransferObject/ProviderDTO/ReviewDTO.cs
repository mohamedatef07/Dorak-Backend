using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ReviewDTO
    {
        public string Providerid { get; set; }
      public decimal Rating { get; set; }
      public string Description { get; set; }
       public string ClientId { get; set; }
    }
}
