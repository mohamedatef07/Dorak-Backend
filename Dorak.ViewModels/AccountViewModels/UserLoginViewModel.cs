using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class UserLoginViewModel
    {

        [Required(ErrorMessage = "This Field is Required")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Invalid UserName ")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This Field is Required")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
