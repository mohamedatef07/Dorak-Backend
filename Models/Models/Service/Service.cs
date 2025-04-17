using Dorak.Models;
using Models.Enums;
using System;

namespace Dorak.ViewModels.Service
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<ProviderCenterService> ProviderCenterServices { get; set; }

    }
}
