using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models.ProviderCenterService
{
    public class ProviderCenterService
    {
        public int ProviderCenterServiceId { get; set; }
        public string ServiceName { get; set; }
        public bool IsDeleted { get; set; }
        public int ProviderId { get; set; }
        public virtual Provider Provider { get; set; } 
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}
