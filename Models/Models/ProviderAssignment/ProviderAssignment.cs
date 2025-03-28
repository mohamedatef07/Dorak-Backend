using Models.Enums;

namespace Dorak.Models
{ 
    public class ProviderAssignment
    {
        public int AssignmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public ProviderType AssignmentType { get; set; }
        public bool IsDeleted { get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int CenterId { get; set; }
        public virtual Center Center { get; set; }
    }

}