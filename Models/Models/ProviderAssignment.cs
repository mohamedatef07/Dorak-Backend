using Models.Enums;

namespace Models.Models
{ 
    public class ProviderAssignment
    {
        public int AssignmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public ProviderType AssignmentType { get; set; }
        public string ProviderId { get; set; }
        public Provider Provider { get; set; }
        public int CenterId { get; set; }
        public Center Center { get; set; }
    }

}