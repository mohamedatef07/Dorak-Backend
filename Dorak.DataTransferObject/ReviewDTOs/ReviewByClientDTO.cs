namespace Dorak.DataTransferObject
{
    public class ReviewByClientDTO
    {
        public string ProviderName { get; set; }
        public string Review { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date { get; set; }
    }
}


