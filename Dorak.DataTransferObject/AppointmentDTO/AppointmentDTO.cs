using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class AppointmentDTO
    {
        public int appointmentId {  get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }= AppointmentStatus.Pending;
        public ClientType clientType { get; set; }
        public decimal Fees { get; set; }
        public decimal AdditionalFees { get; set; } = decimal.Zero;
        public TimeOnly EstimatedTime { get; set; }
        public TimeOnly ExactTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool? IsChecked { get; set; }
        public string? OperatorId { get; set; }
        public int? LiveQueueId { get; set; }   // CHANGED
        public string? ProviderId { get; set; }
        public int? CenterId { get; set; }
        public int? ServiceId { get; set; }
        public int ShiftId { get; set; }
        public string? UserId { get; set; }
        public int? TemporaryClientId { get; set; }
        public string ProviderName { get; set; }
        public decimal Rate { get; set; }
        public string Specialization { get; set; }

    }
}
