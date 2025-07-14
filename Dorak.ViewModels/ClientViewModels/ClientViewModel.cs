using Models.Enums;

namespace Dorak.ViewModels
{
    public class ClientViewModel
    {
        public string ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType? Gender { get; set; } = GenderType.none;
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
    }
}
