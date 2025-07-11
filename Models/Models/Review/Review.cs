namespace Dorak.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public bool IsDeleted { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
