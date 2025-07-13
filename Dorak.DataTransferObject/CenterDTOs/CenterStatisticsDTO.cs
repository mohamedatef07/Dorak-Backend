using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class CenterStatisticsDTO
    {
        public string CenterName { get; set; }
        public int ProvidersCount { get; set; }
        public int OperatorsCount { get; set; }
        public int AppointmentsCount {get; set;}
        public decimal TotalRevenue { get; set; } 
    }
}