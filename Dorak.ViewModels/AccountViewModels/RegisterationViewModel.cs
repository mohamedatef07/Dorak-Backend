using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class RegisterationViewModel
    {
        // From UserRegisterViewModel 
        //UPDATED
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        // Shared fields (present in Client, Provider, AdminCenter)
        // Duplicated in ClientRegisterViewModel, ProviderRegisterViewModel, and AdminCenterViewModel
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public GenderType Gender { get; set; }
        public DateOnly BirthDate { get; set; }


        // From ClientRegisterViewModel
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public IFormFile? Image { get; set; }


        // From ProviderRegisterViewModel
        public string? Specialization { get; set; }
        public string? Bio { get; set; }
        public int? ExperienceYears { get; set; }
        public ProviderType? ProviderType { get; set; }
        public string? LicenseNumber { get; set; }
        public int? EstimatedDuration { get; set; }
        public decimal? Rate { get; set; }
        public ProviderTitle providerTitle { get; set; }

        // From AdminCenterViewModel
        // FirstName, LastName, Gender, and Image are already included above.

        // From CenterDTO_
        public string? CenterName { get; set; }
        public string? ContactNumber { get; set; }
        public string? CenterStreet { get; set; }
        public string? CenterCity { get; set; }
        public string? CenterGovernorate { get; set; }
        public string? CenterCountry { get; set; }
        public string? CenterEmail { get; set; }
        public string? WebsiteURL { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? MapURL { get; set; }



        // From Operator
        public int? CenterId { get;set; }
    }
}
