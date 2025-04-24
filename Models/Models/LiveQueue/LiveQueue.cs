namespace Dorak.Models
{
    public class LiveQueue
    {
        public int LiveQueueId { get; set; }
        public int CurrentQueuePosition { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime EstimatedTime { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
        public string OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }
    }
}