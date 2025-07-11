namespace Dorak.DataTransferObject
{
    public class ReviewByProviderDTO
    {

        public string ClientName { get; set; }
        public string Review { get; set; }
        public string ProviderName { get; set; }
        public string ClientId { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date { get; set; }
    }

}

