using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject
{

    public class UpdateProviderProfileDTO {
    
    
        public string ProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; } 
        public string Phone { get; set; } 
        public GenderType Gender { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Image { get; set; }
    }

}



