namespace Dorak.DataTransferObject.ReviewDTOs
{
    public class AddReviewDTO
    {
        public string ProviderId { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
    }
}
