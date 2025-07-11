namespace Dorak.DataTransferObject
{
    public class ProviderCenterServiceDTO
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public string ServiceName { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
    }
}
