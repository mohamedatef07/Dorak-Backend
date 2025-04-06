using Models.Enums;

namespace Dorak.Models
{
    public class AdminCenterManagement
    {
        public int AdminCenterManagementId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string? Image { get; set; }
        public bool IsDeleted   { get; set; }
        public string AdminId { get; set; }
        public virtual User Admin { get; set; }
        public int CenterId { get; set; }
        public virtual Center? Center { get; set; }
    }
}
