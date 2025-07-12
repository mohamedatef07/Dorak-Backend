namespace Dorak.DataTransferObject
{
    public class ReviewByClientDTO
    {
        public int ReviewId { get; set; }
        public string ProviderName { get; set; }
        public string Review { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date { get; set; }
    }
}


