using Dorak.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class CheckoutRequest
    {
        public int AppointmentId { get; set; }
        public string ClientId { get; set; }
        public string StripeToken { get; set; }
        public decimal Amount { get; set; }
    }
}
