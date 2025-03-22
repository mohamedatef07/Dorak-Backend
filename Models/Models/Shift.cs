using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class Shift
    {

        public int ShiftId { get; set; }

        public ShiftType shiftType { get; set; }

        public int ProviderAssignmentId { get; set; }
        public virtual ProviderAssignment ProviderAssignment {get; set;}

        public int OperatorId { get; set; }
        public virtual Operator Operator { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
