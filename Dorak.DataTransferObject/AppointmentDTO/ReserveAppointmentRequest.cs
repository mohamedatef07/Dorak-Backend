using Dorak.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ReserveAppointmentRequest
    {
        public AppointmentDTO AppointmentDTO { get; set; }
        public string StripeToken { get; set; }
        public decimal Amount { get; set; }
    }
}
