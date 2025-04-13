using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class GetProviderScheduleShiftsInfoDTO
    {
        public ShiftType ShiftType { get; set; }
        public int TotalAppointments {  get; set; }
        public int ApprovedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public decimal AverageEstimatedTime { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
