using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ClientProfileDTO
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public string? Image { get; set; }
        public string Phone { get; set; }

        public ICollection<AppointmentForClientProfileDTO> Appointments { get; set; }
    }
}
