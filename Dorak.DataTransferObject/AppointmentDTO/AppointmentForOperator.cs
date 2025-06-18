using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class AppointmentForOperator
    {
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }

        public AppointmentStatus AppointmentStatus { get; set; }
        public AppointmentType AppointmentType { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ClientType ClientType { get; set; }

        public decimal Fees { get; set; }
        public decimal AdditionalFees { get; set; }

        public TimeOnly? ArrivalTime { get; set; }
        public TimeOnly EstimatedTime { get; set; }
        public int EstimatedDuration { get; set; }
        public TimeOnly ExactTime { get; set; }
        public TimeOnly EndTime { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactInfo { get; set; }

        public string? OperatorId { get; set; }
        public string? ProviderId { get; set; }
        public int? ServiceId { get; set; }
        public int? CenterId { get; set; }

        public int ShiftId { get; set; }

        public string? UserId { get; set; }
        public int? TemporaryClientId { get; set; }

        public bool IsChecked { get; set; }
    }
}
