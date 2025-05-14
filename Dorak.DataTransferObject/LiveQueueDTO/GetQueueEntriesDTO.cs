using Dorak.Models;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GetQueueEntriesDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public int? CurrentQueuePosition { get; set; }
        public TimeOnly? ArrivalTime { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public ClientType Type { get; set; }
        public QueueAppointmentStatus Status { get; set; }

    }
}
