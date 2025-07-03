using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class AppointmentCardDTO
    {
        public int AppointmentId { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public TimeOnly EstimatedTime { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string? ProviderImage { get; set; }
        public decimal ProviderRate { get; set; }
        public string Specialization { get; set; }
    }
}
