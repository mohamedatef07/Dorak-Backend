using System;
using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Dorak.ViewModels.AccountViewModels
{
    public class ClientRegisterViewModel
    {
        // Basic client info
        [Required(ErrorMessage = "This field is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
         public GenderType Gender { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Country { get; set; }
        public string Image { get; set; }
    }
}
