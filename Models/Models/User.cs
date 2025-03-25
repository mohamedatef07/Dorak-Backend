using Microsoft.AspNetCore.Identity;

namespace Models.Models
{
    public class User : IdentityUser
    {
        public virtual Client? Client { get; set; }
        public virtual Provider? Provider { get; set; }
        public virtual ICollection<AdminCenterManagement> AdminCentersManagement { get; set; }

    }
}
