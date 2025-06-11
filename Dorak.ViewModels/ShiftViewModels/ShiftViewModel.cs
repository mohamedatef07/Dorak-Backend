using Models.Enums;
using System;

namespace Dorak.ViewModels
{
    public class ShiftViewModel
    {
        public ShiftType ShiftType { get; set; }
        public DateOnly ShiftDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int? MaxPatientsPerDay { get; set; }
        public string OperatorId { get; set; }
    }
}
