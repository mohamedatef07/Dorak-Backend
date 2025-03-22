namespace Models.Models
{
    public class Center
    {
        public int CenterID { get; set; }
        public string CenterName { get; set; }
        public string ContactNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string WebsiteURL { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string MapURL { get; set; }
        public virtual ICollection<AdminCenterManagement> AdminCentersManagement { get; set; }
        public virtual ICollection<CenterService> CenterServices { get; set; }
        public virtual ICollection<ProviderAssignment> ProviderAssignments { get; set; }
        //
    }
}
