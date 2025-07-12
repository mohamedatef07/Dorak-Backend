namespace Dorak.DataTransferObject
{
    public class GetAllProviderReviewsDTO
    {
        public string ClientName { get; set; }
        public string Review { get; set; }
        public string ClientId { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date { get; set; }
    }

}

