using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//ByAmgad
namespace Models.Models
{

    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ConfirmationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Type { get; set; }
        public decimal Fees { get; set; }
        public decimal AdditionalFees { get; set; }
        public DateTime EstimatedTime { get; set; }
        public DateTime ExactTime { get; set; }
        public DateTime EndTime { get; set; }

      
        /// //////////////////////////
      
        //relation in Operator table!!!!!!!
        public int? OperatorId { get; set; }
        public virtual Operator Operator { get; set; }

        public int LiveQueueId { get; set; }
        public virtual LiveQueue LiveQueue { get; set; }

        //relation in Provider table!!!!!!!
        public int ProviderId { get; set; }
        public virtual Provider Provider { get; set; }


        public int CenterId { get; set; }
        public virtual Center Center { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        
        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }


        //in table user !!!!!!!!!!!!!!
        public int? UserId { get; set; }
        public virtual User User { get; set; }

        //in table TemporaryClient !!!!!!!!!!!!!!
        public int? TemporaryClientId { get; set; }
        public virtual TemporaryClient TemporaryClient { get; set; }

        public virtual Payment Payment { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }

    }
}
