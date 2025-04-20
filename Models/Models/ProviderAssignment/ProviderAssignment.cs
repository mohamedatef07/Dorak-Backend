using Models.Enums;

namespace Dorak.Models
{
    public class ProviderAssignment
    {
        public int AssignmentId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public AssignmentType AssignmentType { get; set; }
        public bool IsDeleted { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
        public virtual ICollection<Shift> Shifts { get; set; }

    }

}