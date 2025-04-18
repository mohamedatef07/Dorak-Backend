using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.ViewModels
{
    public class ShiftDTO
    {
        public int ShiftId { get; set; }
        public ShiftType ShiftType { get; set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxPatientsPerDay { get; set; }
        public int CenterId { get; set; }
        public string OperatorId { get; set; }
        public string ProviderId { get; set; }
    }
}
