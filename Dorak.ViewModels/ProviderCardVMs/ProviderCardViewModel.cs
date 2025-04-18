using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.ViewModels.DoctorCardVMs
{
    
        public class ProviderCardViewModel
        {
            public string FullName { get; set; }
            public string Specialization { get; set; }
            public decimal Rate { get; set; }
            public int EstimatedDuration { get; set; }
            public string City { get; set; }
            public decimal Price { get; set; }
        }

}


