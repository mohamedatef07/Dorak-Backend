namespace Models.Models
{
    public class ProviderService
    {
        public int ProviderServiceId { get; set; }
        public decimal CustomPrice { get; set; }
        public string ProviderId { get; set; }
        public Provider Provider { get; set; }
        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}