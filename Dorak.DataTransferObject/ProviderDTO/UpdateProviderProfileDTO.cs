using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class UpdateProviderProfileDTO {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } 
        public string Phone { get; set; } 
        public DateOnly BirthDate { get; set; }
        public IFormFile? Image { get; set; }
    }
}



