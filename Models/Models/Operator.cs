using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Models.Models
{
    public class Operator
    {
        public string OperatorId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
        public virtual ICollection<LiveQueue> LiveQueues { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
