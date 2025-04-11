using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class EditProviderViewModel
    {
        public string ProviderId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string Specialization { get; set; }
        public int? ExperienceYears { get; set; }
        public string Availability { get; set; }
        public string LicenseNumber { get; set; }
        public int EstimatedDuration { get; set; }

    }
}
