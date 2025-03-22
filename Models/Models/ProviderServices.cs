namespace Models.Models
{
    public class ProviderService
    {
        public int ID { get; set; }
        public decimal CustomPrice { get; set; }
        public int ProviderID { get; set; }
        public Provider Provider { get; set; }
        public int ServiceID { get; set; }
        public int ServiceID { get; set; }
        public Service Service { get; set; }
    }
}