using Models.Enums;

namespace Models.Models
{ 
    public class ProviderAssignment
    {
        public int AssignmentID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public ProviderType AssignmentType { get; set; }
        public int ProviderID { get; set; }
        public Provider Provider { get; set; }
        public int CenterID { get; set; }
        public Center Center { get; set; }
    }

}