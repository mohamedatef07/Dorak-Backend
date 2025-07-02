namespace Dorak.DataTransferObject.ClientDTO
{
    public class GetClientAppointmentStatisticsDTO
    {
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
    }
}
