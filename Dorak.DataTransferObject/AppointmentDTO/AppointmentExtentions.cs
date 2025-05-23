using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dorak.DataTransferObject;

using Dorak.Models;

namespace Dorak.ViewModels
{
    public static class AppointmentExtentions
    {
        public static AppointmentDTO AppointmentToAppointmentDTO(this Appointment appointment)
        {
            return new AppointmentDTO
            {
                appointmentId = appointment.AppointmentId,
                FirstName = appointment.User.Client.FirstName,
                LastName = appointment.User.Client.LastName,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentStatus = appointment.AppointmentStatus,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                clientType = appointment.ClientType,
                Fees = appointment.Fees,
                AdditionalFees = appointment.AdditionalFees,
                EstimatedTime = appointment.EstimatedTime,
                ExactTime = appointment.ExactTime,
                EndTime = appointment.EndTime,
                IsChecked = appointment.IsChecked,
                OperatorId = appointment.OperatorId,
                ProviderId = appointment.ProviderCenterService.ProviderId,
                CenterId = appointment.ProviderCenterService.CenterId,
                ServiceId = appointment.ProviderCenterService.ServiceId,
                ShiftId = appointment.ShiftId,
                UserId = appointment.UserId,
                TemporaryClientId = appointment.TemporaryClientId

            };
        }
        public static Appointment AppointmentDTOToAppointment(this AppointmentDTO appointmentDTO)
        {


            return new Appointment
            {
                AppointmentId = appointmentDTO.appointmentId,
                AppointmentDate = appointmentDTO.AppointmentDate,
                AppointmentStatus = appointmentDTO.AppointmentStatus,
                CreatedAt = appointmentDTO.CreatedAt,
                UpdatedAt = appointmentDTO.UpdatedAt,
                ClientType = appointmentDTO.clientType,
                Fees = appointmentDTO.Fees,
                AdditionalFees = appointmentDTO.AdditionalFees,
                EstimatedTime = appointmentDTO.EstimatedTime,
                ExactTime = appointmentDTO.ExactTime,
                EndTime = appointmentDTO.EndTime,
                IsChecked = appointmentDTO.IsChecked ?? false,
                OperatorId = appointmentDTO.OperatorId,
                ShiftId = appointmentDTO.ShiftId,
                UserId = appointmentDTO.UserId,
                TemporaryClientId = appointmentDTO.TemporaryClientId ?? null
            };
        }
        public static Appointment reserveApointmentDTOToAppointment(this ReserveApointmentDTO reserveApointmentDTO)
        {
            return new Appointment
            {
                AppointmentDate = reserveApointmentDTO.AppointmentDate,
                AppointmentStatus = reserveApointmentDTO.AppointmentStatus,
                CreatedAt = reserveApointmentDTO.CreatedAt,
                UpdatedAt = reserveApointmentDTO.UpdatedAt,
                ClientType = reserveApointmentDTO.clientType,
                Fees = reserveApointmentDTO.Fees,
                OperatorId = reserveApointmentDTO.OperatorId,
                ShiftId = reserveApointmentDTO.ShiftId,
                UserId = reserveApointmentDTO.UserId,
                TemporaryClientId = reserveApointmentDTO.TemporaryClientId ?? null
            };
        }
        
        public static AppointmentForClientProfileDTO AppointmentToAppointmentForClientProfileDTO(this Appointment appointment)
        {

            return new AppointmentForClientProfileDTO
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                ProviderId = appointment.ProviderCenterService.ProviderId,
                ProviderName = $"{appointment.ProviderCenterService.Provider.FirstName } {appointment.ProviderCenterService.Provider.LastName}",
                Rate =appointment.ProviderCenterService.Provider.Rate,
                Specialization = appointment.ProviderCenterService.Provider.Specialization

            };
        }
    }
}
