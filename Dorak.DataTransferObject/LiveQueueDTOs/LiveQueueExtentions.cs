using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.ViewModels;

namespace Dorak.DataTransferObject
{
    public static class LiveQueueExtentions
    {
        public static LiveQueueDTO AppointmentDTO_LiveQueueDTO (this AppointmentDTO appointmentDTO)
        {
            return new LiveQueueDTO
            {
                ShiftId = appointmentDTO.ShiftId,
                OperatorId = appointmentDTO.OperatorId,
                AppointmentId = appointmentDTO.appointmentId,
                ArrivalTime = null,
                EstimatedTime = appointmentDTO.EstimatedTime,
                CurrentQueuePosition = null,
                //EstimatedDuration = ,
                //Status = appointmentDTO.



            };
        }
    }
}
