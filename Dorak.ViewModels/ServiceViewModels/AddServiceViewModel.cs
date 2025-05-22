using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class AddServiceViewModel
    {
        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "Service Name")]
        public string ServiceName { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        public int Priority { get; set; }
    }

}

