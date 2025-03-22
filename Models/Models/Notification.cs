using System;

namespace Models.Models
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; } 
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string DeliveryMethod { get; set; } 

        public int UserID { get; set; } 
        public User User { get; set; }
        public int PaymentID { get; set; }
        public virtual Payment Payment { get; set; }

        public int AppointmentID { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
