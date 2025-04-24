using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; } 
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }

    }
}
