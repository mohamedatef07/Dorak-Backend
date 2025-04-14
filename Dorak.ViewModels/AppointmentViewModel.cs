using Dorak.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.ViewModels
{
    public class AppointmentViewModel
    {

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public AppointmentStatus AppointmentStatus { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100, ErrorMessage = "Type cannot exceed 100 characters.")]
        public string Type { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Fees must be a positive number.")]
        public decimal Fees { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Additional Fees must be a positive number.")]
        public decimal AdditionalFees { get; set; }

        public TimeOnly EstimatedTime { get; set; }
        public TimeOnly ExactTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [Required]
        public string OperatorId { get; set; }

        [Required]
        public int LiveQueueId { get; set; }

        [Required]
        public string ProviderId { get; set; }

        [Required]
        public int CenterId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public int ShiftId { get; set; }

        [Required]
        public string UserId { get; set; }

        public int? TemporaryClientId { get; set; }
    }
}
