using System;
using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class ProviderRegisterViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Value must be at least 3 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Value must be at least 3 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Specialization { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Value must be at least 3 characters")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int? ExperienceYears { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public ProviderType? ProviderType { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public GenderType Gender { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public DateOnly? BirthDate { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string? Image { get; set; }
        public string Availability { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal Rate { get; set; }

    }
}
