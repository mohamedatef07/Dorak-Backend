using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class RescheduleAssignmentViewModel
    {
        public string ProviderId { get; set; }
        public int CenterId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<int>? WorkingDays { get; set; } 
        public List<ShiftViewModel>? Shifts { get; set; }
    }

}
