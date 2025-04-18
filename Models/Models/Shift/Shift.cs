using Models.Enums;
using System;


namespace Dorak.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public ShiftType ShiftType { get; set; }
        public int ProviderAssignmentId { get; set; }
        public DateOnly ShiftDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int EstimatedDuration { get; set; }
        public int MaxPatientsPerDay { get; set; }
        public bool IsDeleted { get; set; }
        public virtual ProviderAssignment ProviderAssignment {get; set;}
        public string OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual LiveQueue LiveQueue { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
