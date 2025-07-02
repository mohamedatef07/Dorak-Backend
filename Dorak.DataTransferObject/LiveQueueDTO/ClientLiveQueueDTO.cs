using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool IsCurrentClient {  get; set; }
    }
}
