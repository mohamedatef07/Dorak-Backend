using Microsoft.AspNetCore.Identity;

namespace Dorak.Models
{
    public class User : IdentityUser
    {
        public virtual Client? Client { get; set; }
        public virtual Provider? Provider { get; set; }
        public virtual Operator? Operator { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual AdminCenterManagement? AdminCentersManagement { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}