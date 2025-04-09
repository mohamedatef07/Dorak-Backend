using Models.Enums;

namespace Dorak.Models
{
    public class ProviderCenterService
    {
        public int ProviderCenterServiceId { get; set; }

        public int ProviderServiceId { get; set; }
        public virtual ProviderService ProviderService { get; set; }

        public int CenterId { get; set; }
        public virtual Center Center { get; set; }

        public int Duration { get; set; }
        public decimal Price { get; set; }
        public ServicePriority Priority { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}