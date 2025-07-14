using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class ClientLiveQueueDTO
    {

        public int? CurrentQueuePosition { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public ClientType Type { get; set; }
        public QueueAppointmentStatus Status { get; set; }
        public int AppointmentId { get; set; }
        public bool IsCurrentClient { get; set; }
        public int EstimatedDuration { get; set; }
    }
}
