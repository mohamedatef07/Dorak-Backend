using Models.Enums;

namespace Dorak.Models
{
    public class Center
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public string ContactNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string WebsiteURL { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string MapURL { get; set; }
        public bool IsDeleted { get; set; }
        public CenterStatus CenterStatus { get; set; }
        public virtual ICollection<AdminCenterManagement> AdminCentersManagement { get; set; }
        public virtual ICollection<ProviderCenterService> ProviderCenterServices { get; set; }
        public virtual ICollection<ProviderAssignment> ProviderAssignments { get; set; }
    }
}
