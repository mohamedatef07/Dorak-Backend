using Models.Enums;

namespace Dorak.Models
{
    public class ProviderCenterService
    {
        public int ProviderCenterServiceId { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public int Priority { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}