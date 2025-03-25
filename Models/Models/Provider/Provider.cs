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
<<<<<<< HEAD:Models/Models/Provider/Provider.cs
        public GenderType Gender { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
=======
        public Gender Gender { get; set; }
        public string Address { get; set; }
        //public string Street { get; set; } 
        //public string City { get; set; }   
        //public string Governorate { get; set; } 
        //public string Country { get; set; }
>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Models/Provider.cs
        public DateOnly BirthDate { get; set; }
        public string? PicName { get; set; }
        public string Availability { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal Rate { get; set; }
<<<<<<< HEAD:Models/Models/Provider/Provider.cs
        public virtual ICollection<ProviderCertification> Certifications { get; set; }
        public virtual ICollection<ProviderAssignment> ProviderAssignments { get; set; }
        public virtual ICollection<ProviderService> ProviderServices { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; } 
=======

        public virtual ICollection<ProviderCertifications> Certifications { get; set; }



>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Models/Provider.cs
    }
}
