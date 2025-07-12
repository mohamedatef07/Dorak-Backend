using Dorak.ViewModels;
using Dorak.DataTransferObject;
using System.Collections.Generic;

namespace Dorak.DataTransferObject
{
    public class CenterStatisticsDTO
    {
        public int ProvidersCount { get; set; }
        public int OperatorsCount { get; set; }
        public int AppointmentsCount { get; set; }
        public decimal TotalRevenue {  get; set; }
    }
}
