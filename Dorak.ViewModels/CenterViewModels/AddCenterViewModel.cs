using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class AddCenterViewModel
    {
        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "Center Name")]
        public string CenterName { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Governorate { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string City { get; set; }
        [Required(ErrorMessage = "This Field is Required")]
        public string Street { get; set; }
        public string Email { get; set; }
        [Display(Name = "Website URL")]
        public string WebsiteURL { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        [Display(Name = "Map URL")]
        public string MapURL { get; set; }
        public CenterStatus CenterStatus { get; set; }
    }
}
