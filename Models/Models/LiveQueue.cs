using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class LiveQueue
    {
        public int LiveQueueId { get; set; }
        public int CurrentQueuePosition { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime EstimatedTime { get; set; }
        public int EstimatedDuration { get; set; }
        public string Status {  get; set; }
        public int Capacity { get; set; }
        public string OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
