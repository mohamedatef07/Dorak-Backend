using Models.Enums;

namespace Dorak.DataTransferObject.ProviderDTO
{
    public class FilterProviderDTO
    {
        public ProviderTitle? Title { get; set; }
        public GenderType? Gender { get; set; }

        public string? City { get; set; }
        public string? SearchText { get; set; }
        public string? Specialization { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public decimal? MinRate { get; set; }
        public decimal? MaxRate { get; set; }

        public DateOnly? AvailableDate { get; set; }

    }
}
