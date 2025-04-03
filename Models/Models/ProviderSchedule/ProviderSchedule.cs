using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.Models
{
    public class ProviderSchedule
    {
        public int ProviderScheduleId { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatientsPerDay { get; set; }
    }
}
