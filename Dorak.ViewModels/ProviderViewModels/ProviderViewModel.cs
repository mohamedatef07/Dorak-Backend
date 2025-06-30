using Dorak.Models;
using Microsoft.AspNetCore.Http;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Dorak.ViewModels
{
    
    public class ProviderViewModel
    {
       
        public string ProviderId { get; set; }
        public int AssignmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public int? ExperienceYears { get; set; }
        public string LicenseNumber { get; set; }
        public GenderType Gender { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Image { get; set; }
        public int EstimatedDuration { get; set; }
        public ProviderStatus Status { get; set; } = ProviderStatus.Offline;
        public DateOnly? AddDate { get; set; }
        public string? PhoneNumber { get; set; } 
        public string? Email { get; set; }

        //public ProviderViewModel(Provider provider)
        //{
        //    PhoneNumber = provider.User?.PhoneNumber;
        //}
    }
}
