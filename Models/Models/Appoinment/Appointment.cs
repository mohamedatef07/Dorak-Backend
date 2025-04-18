using Models.Enums;

namespace Dorak.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ClientType ClientType { get; set; }
        public decimal Fees { get; set; }
        public decimal AdditionalFees { get; set; }
        public TimeOnly EstimatedTime { get; set; }
        public TimeOnly ExactTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        //public int LiveQueueId { get; set; }
        //public virtual LiveQueue LiveQueue { get; set; }
        public int ProviderCenterServiceId { get; set; }
        public virtual ProviderCenterService ProviderCenterService { get; set; }
        //public int ShiftId { get; set; }
        //public virtual Shift Shift { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int? TemporaryClientId { get; set; }
        public bool IsChecked { get; set; } = false;
        public virtual TemporaryClient TemporaryClient { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
