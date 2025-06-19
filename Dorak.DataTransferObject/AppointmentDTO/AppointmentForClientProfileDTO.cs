using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class AppointmentForClientProfileDTO
    {
        public int AppointmentId { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string? ProviderIamge{ get; set; }
        public decimal Rate { get; set; }
        public string Specialization { get; set; }
        public DateOnly AppointmentDate {  get; set; }

    }
}
