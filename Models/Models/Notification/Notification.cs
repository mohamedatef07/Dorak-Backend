using System;

namespace Dorak.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }=false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.Now.AddDays(14);
        public string UserId { get; set; } 
        public virtual User User { get; set; }
        public int? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }
        public int? AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
