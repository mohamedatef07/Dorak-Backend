using System;
using System.Collections.Generic;


namespace Models.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int AppointmentID { get; set; }
        public int ClientID { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionID { get; set; }
        public string PaymentStatus { get; set; }
        public string RefundStatus { get; set; }
        public DateTime TransactionDate { get; set; }

        
        public Appointment Appointments { get; set; }
        public Client Clients { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
