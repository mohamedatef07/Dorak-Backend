using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GetAllProviderScheduleDetailsDTO
    {
        public int CenterId { get; set; }
        public int ShiftId { get; set; }
        public ShiftType ShiftType { get; set; }
        public int TotalAppointments { get; set; }
        public int ApprovedAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public TimeOnly AverageEstimatedTime { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
