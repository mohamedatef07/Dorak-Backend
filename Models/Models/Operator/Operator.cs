using Models.Enums;

namespace Dorak.Models
{
    public class Operator
    {
        public string OperatorId { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
        public int? CenterId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual Center Center { get; set; }
        public virtual ICollection<LiveQueue> LiveQueues { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<TemporaryClient> TemporaryClients { get; set; }
    }
}
