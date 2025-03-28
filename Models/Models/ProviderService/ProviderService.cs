
namespace Dorak.Models
{
    public class ProviderService
    {
        public int ProviderServiceId { get; set; }
        public decimal CustomPrice { get; set; }
        public bool IsDeleted { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}