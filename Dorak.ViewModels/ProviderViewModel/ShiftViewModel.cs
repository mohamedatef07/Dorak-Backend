using Models.Enums;
using System;

namespace Dorak.ViewModels
{
    public class ShiftViewModel
    {
        public int ProviderAssignmentId { get; set; }

        //public string OperatorId { get; set; }
        public ShiftType ShiftType { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatientsPerDay { get; set; }
    }
}
