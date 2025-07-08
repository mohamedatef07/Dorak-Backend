using Models.Enums;

namespace Dorak.DataTransferObject.ProviderDTO
{
    public class FilterProviderDTO
    {
        public List<ProviderTitle>? Titles { get; set; }
        public List<GenderType>? Genders { get; set; }

        public List<string>? Cities { get; set; }
        public string? SearchText { get; set; }
        public List<string>? Specializations { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public decimal? MinRate { get; set; }
        public decimal? MaxRate { get; set; }

        public DateOnly? AvailableDate { get; set; }

    }
}
