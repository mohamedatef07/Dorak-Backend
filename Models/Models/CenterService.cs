namespace Models.Models
{
    public class CenterService
    {
        public int CenterServiceId { get; set; }
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
    }
}
