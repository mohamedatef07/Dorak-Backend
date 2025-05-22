using System;
using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Dorak.ViewModels.AccountViewModels
{
    public class RegisterationViewModel
    {
        // From UserRegisterViewModel 
        //UPDATED
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }


        // Shared fields (present in Client, Provider, AdminCenter)
        // Duplicated in ClientRegisterViewModel, ProviderRegisterViewModel, and AdminCenterViewModel
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }


        // From ClientRegisterViewModel
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public string? Image { get; set; }


        // From ProviderRegisterViewModel
        public string? Specialization { get; set; }
        public string? Bio { get; set; }
        public int? ExperienceYears { get; set; }
        public string? ProviderType { get; set; }
        public string? LicenseNumber { get; set; }
        public string? PicName { get; set; }
        public string? Availability { get; set; }
        public int? EstimatedDuration { get; set; }
        public decimal? Rate { get; set; }


        // From AdminCenterViewModel
        // FirstName, LastName, Gender, and Image are already included above.
    }
}
