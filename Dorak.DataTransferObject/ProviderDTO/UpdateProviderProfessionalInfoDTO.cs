using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class UpdateProviderProfessionalInfoDTO
    {
        public string ProviderId { get; set; }
        public string Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string LicenseNumber { get; set; }
        public string Bio { get; set; } 
    }
}
