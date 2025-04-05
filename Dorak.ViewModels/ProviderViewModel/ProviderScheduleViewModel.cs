namespace Dorak.ViewModels
{
    public class ProviderScheduleViewModel
    {
        public string ProviderId { get; set; }
        public int CenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxPatientsPerDay { get; set; }
    }
}
