using System;

namespace Models.Models
{
    public class TemporaryClient
    {
        public int TempClientId {  get; set; }
        public string? ContactInfo { get; set; }
        public string TempCode { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
