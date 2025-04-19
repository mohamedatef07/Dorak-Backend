using Dorak.Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GetProviderBookingInfoDTO
    {
        public int CenterId { get; set; }
        public ShiftType ShiftType { get; set; }
        public DateOnly Date {  get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

    }
}
