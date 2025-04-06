using Models.Enums;

namespace Dorak.Models
{
    public class Provider
    {
        public string ProviderId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Description { get; set; }
        public int? ExperienceYears { get; set; }
        public string LicenseNumber {  get; set; }
        public GenderType? Gender { get; set; } = GenderType.none;
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public DateOnly BirthDate { get; set; }
        public UserStatus? Status { get; set; } = UserStatus.none;
        public ProviderType providerType { get; set; }
        public string? PicName { get; set; }
        public string Availability { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal Rate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<ProviderCertification> Certifications { get; set; }
        public virtual ICollection<ProviderAssignment> ProviderAssignments { get; set; }
        public virtual ICollection<ProviderService> ProviderServices { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } 
    }
}
