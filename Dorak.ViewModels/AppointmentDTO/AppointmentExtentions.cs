using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class AppointmentExtentions
    {
        public static AppointmentDTO AppointmentToAppointmentDTO(this Appointment appointment)
        {
            return new AppointmentDTO
            {
                FirstName = appointment.User.Client.FirstName,
                LastName = appointment.User.Client.LastName,
                clientType = appointment.ClientType,
                EstimatedTime = appointment.EstimatedTime,
                Fees = appointment.Fees,
                AppointmentStatus = appointment.AppointmentStatus,
                AdditionalFees = appointment.AdditionalFees,
                AppointmentDate = appointment.AppointmentDate,

            };
        }
    }
}
