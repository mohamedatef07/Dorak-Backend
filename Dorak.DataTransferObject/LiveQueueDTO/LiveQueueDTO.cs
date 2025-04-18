using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models.Enums;

namespace Dorak.DataTransferObject
{
    public class LiveQueueDTO
    {
        public int? CurrentQueuePosition { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public TimeOnly EstimatedTime { get; set; }
        public int? EstimatedDuration { get; set; }
        public QueueAppointmentStatus Status { get; set; }
        //public int? Capacity { get; set; }
        public string OperatorId { get; set; }
        public int AppointmentId { get; set; }
        public int ShiftId { get; set; }
    }
}
