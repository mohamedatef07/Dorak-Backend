using Models.Enums;
using System.Reflection;

namespace Models.Models
{
    public class AdminCenterManagement
    {
        public int AdminCenterManagementID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
        public string AdminID { get; set; }
        public virtual User Admin { get; set; }
        public int CenterID { get; set; }
        public virtual Center Center { get; set; }
    }
}
