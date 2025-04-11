using Dorak.Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class GetProviderBookingInfoViewModel
    {
        public int CenterId { get; set; }
        public List<int> ServiceId { get; set; }
        public ShiftType ShiftType { get; set; }
        public string Date {  get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}
