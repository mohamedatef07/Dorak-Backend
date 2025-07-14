using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class GetAllCenterShiftAndServicesDTO
    {
        public string ProviderName { get; set; }
        public int ShiftId { get; set; }
        public ShiftType shiftType { get; set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal fees { get; set; }
        public List<ServicesDTO> Services { get; set; }
        public string ProviderId { get; set; } = string.Empty;
        public int AppointmentCount { get; set; }
    }
}
