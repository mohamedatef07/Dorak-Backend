using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class ReserveApointmentDTO
    {
        public DateOnly AppointmentDate { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; } = AppointmentStatus.Pending;
        public AppointmentType AppointmentType { get; set; } = AppointmentType.Normal;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ClientType clientType { get; set; } = ClientType.Normal;
        public decimal Fees { get; set; }
        public string? OperatorId { get; set; }
        public string? ProviderId { get; set; }
        public int? CenterId { get; set; }
        public int? ServiceId { get; set; }
        public int ShiftId { get; set; }
        public string? UserId { get; set; }
        public int? TemporaryClientId { get; set; }
    }
}
