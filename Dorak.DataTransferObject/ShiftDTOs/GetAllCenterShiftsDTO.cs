using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GetAllCenterShiftsDTO
    {
        public string ProviderName { get; set; }
        public int ShiftId { get; set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public ShiftType  ShiftType { get; set; }
    }


}
