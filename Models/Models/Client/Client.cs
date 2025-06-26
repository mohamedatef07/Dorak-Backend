using Models.Enums;

namespace Dorak.Models
{
    public class Client
    {
        public string ClientId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType? Gender { get; set; } = GenderType.none;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public DateOnly BirthDate { get; set; }
        public UserStatus? Status { get; set; } = UserStatus.none;
        public string? Image {  get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Review> Reviews { get; set; }

    }
}
