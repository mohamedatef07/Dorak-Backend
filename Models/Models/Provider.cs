using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;
using Models.Models;

namespace Models.Models
{
    public class Provider
    {
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Description { get; set; }
        public int? ExperienceYears { get; set; }
        public string LicenseNumber {  get; set; }
        public GenderType Gender { get; set; }
        public string Address { get; set; }
        //public string Street { get; set; } 
        //public string City { get; set; }   
        //public string Governorate { get; set; } 
        //public string Country { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? PicName { get; set; }
        public string Availability { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal Rate { get; set; }

        public virtual ICollection<ProviderCertifications> Certifications { get; set; }

    }
}
