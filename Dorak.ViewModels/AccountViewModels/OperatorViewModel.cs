using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class OperatorViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Value must be at least 3 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Value must be at least 3 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
    }
}
