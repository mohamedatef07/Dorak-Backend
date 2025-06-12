using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Models.Enums;

namespace Dorak.DataTransferObject.ProviderDTO
{
    public class ProviderProfileDTO
    {
        public string ID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public GenderType Gender { get; set; }
        public DateOnly BirthDate { get; set; }

        public string Specialization { get; set; }
        public int? Experience { get; set; }
        public string MedicalLicenseNumber { get; set; }
        public string About { get; set; }
        public string Image { get; set; }

        public IFormFile ImageFile { get; set; } 

        public string Role { get; set; }

    }
}
