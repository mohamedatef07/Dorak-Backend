using System;
using System.Collections.Generic;


namespace Models.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionID { get; set; }
        public string PaymentStatus { get; set; }
        public string RefundStatus { get; set; }
        public DateTime TransactionDate { get; set; }

        
        public int AppointmentID { get; set; }
        public virtual Appointment Appointment { get; set; }
        public string ClientID { get; set; }
        public virtual User Client { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
