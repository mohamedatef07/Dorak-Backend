namespace Dorak.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ConfirmationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Type { get; set; }
        public decimal Fees { get; set; }
        public decimal AdditionalFees { get; set; }
        public DateTime EstimatedTime { get; set; }
        public DateTime ExactTime { get; set; }
        public DateTime EndTime { get; set; }
        public string OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        public int LiveQueueId { get; set; }
        public virtual LiveQueue LiveQueue { get; set; }
        public int ProviderCenterServiceId { get; set; }
        public virtual ProviderCenterService ProviderCenterService { get; set; }
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int? TemporaryClientId { get; set; }
        public virtual TemporaryClient TemporaryClient { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
