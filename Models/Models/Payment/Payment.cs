using System;

namespace Dorak.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public string RefundStatus { get; set; }
        public DateTime TransactionDate { get; set; }
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
        public string ClientId { get; set; }
        public virtual User Client { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
