namespace Dorak.DataTransferObject
{
    public class AddProviderCenterServiceDTO
    {
        public string ProviderId { get; set; }
        public int ServiceId { get; set; }
        public int CenterId { get; set; }
        public decimal Price { get; set; }
        public int Priority { get; set; }

    }
}
