using System;

namespace Dorak.Models
{
    public class TemporaryClient
    {
        public int TempClientId {  get; set; }
        public string? ContactInfo { get; set; }
        public string TempCode { get; set; }
        public bool IsDeleted { get; set; }
        public string? OperatorId { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
