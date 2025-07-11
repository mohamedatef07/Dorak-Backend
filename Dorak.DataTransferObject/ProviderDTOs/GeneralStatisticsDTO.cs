using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GeneralStatisticsDTO
    {
        public int TotalAppointments { get; set; }
        public int TotalUrgentCases { get; set; }
        public int PatientsInQueue { get; set; }
        public decimal AverageEstimatedTime { get; set; }
        public int PatientsTreatedToday { get; set; }
    }
}
