namespace Models.Models
{
    public class AdminCenterManagement
    {
        public int AdminCenterManagementID { get; set; }
        public string AdminID { get; set; }
        public virtual User User { get; set; }
        public int CenterID { get; set; }
        public virtual Center Center { get; set; }
    }
}
