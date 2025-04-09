using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class GetProviderBookingInfoViewModel
    {
        public string CenterName { get; set; }
        public string ServiceName { get; set; }
        public decimal BasePrice { get; set; }
        public ShiftType ShiftType { get; set; }
    }
}
