using Microsoft.AspNetCore.Http;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels.AccountViewModels
{
    public class NewClientViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        public string UserName { get; set; }

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

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Value must be at least 6 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Value must be at least 8 characters")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public GenderType Gender { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public DateOnly BirthDate { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
        public string? Country { get; set; }
        public string? Image { get; set; }


    }
}
