using Models.Enums;
using System;

namespace Dorak.ViewModels.ShiftViewModel
{
    public class ShiftViewModel
    {
        public int ProviderAssignmentId { get; set; }
        public ShiftType ShiftType { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxPatientsPerDay { get; set; }
    }
}
