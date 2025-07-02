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
                TemporaryClientId = appointment.TemporaryClientId,
                ProviderImage = $"{appointment.ProviderCenterService.Provider.Image}",
                ProviderName = $"{appointment.ProviderCenterService.Provider.FirstName} {appointment.ProviderCenterService.Provider.LastName}",
                ProviderRate = appointment.ProviderCenterService.Provider.Rate,
                Specialization = appointment.ProviderCenterService.Provider.Specialization

            };
        }
        public static Appointment AppointmentDTOToAppointment(this AppointmentDTO appointmentDTO)
        {
            return new Appointment
            {
                AppointmentId = appointmentDTO.appointmentId,
                AppointmentDate = appointmentDTO.AppointmentDate,
                AppointmentStatus = appointmentDTO.AppointmentStatus,

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
                TemporaryClientId = appointmentDTO.TemporaryClientId ?? null,
            };
        }
        public static Appointment reserveApointmentDTOToAppointment(this ReserveApointmentDTO reserveApointmentDTO)
        {
            return new Appointment
            {
                AppointmentDate = reserveApointmentDTO.AppointmentDate,
                AppointmentStatus = reserveApointmentDTO.AppointmentStatus,
                AppointmentType = reserveApointmentDTO.AppointmentType,
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
        public static Appointment ToAppointmentFromDTO(this ReserveApointmentDTO dto)
        {
            return new Appointment
            {
                AppointmentDate = dto.AppointmentDate,
                AppointmentStatus = dto.AppointmentStatus,
                AppointmentType = dto.AppointmentType,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt,
                ClientType = dto.clientType,
                Fees = dto.Fees,
                AdditionalFees = dto.AdditionalFees,
                OperatorId = dto.OperatorId,
                ShiftId = dto.ShiftId
            };
        }

        public static AppointmentCardDTO AppointmentToAppointmentCardDTO(this Appointment appointment)
        {

            return new AppointmentCardDTO
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentStatus = appointment.AppointmentStatus,
                EstimatedTime = appointment.EstimatedTime,
                ProviderId = appointment.ProviderCenterService.ProviderId,
                ProviderIamge = $"{appointment.ProviderCenterService.Provider.Image}",
                ProviderName = $"{appointment.ProviderCenterService.Provider.FirstName} {appointment.ProviderCenterService.Provider.LastName}",
                ProviderRate = appointment.ProviderCenterService.Provider.Rate,
                Specialization = appointment.ProviderCenterService.Provider.Specialization
            };
        }

        public static AppointmentForOperator ToReserveAppointmentResultDTO(this Appointment appointment)
        {
            var appop = new AppointmentForOperator
            {
                AppointmentId = appointment.AppointmentId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentStatus = appointment.AppointmentStatus,
                AppointmentType = appointment.AppointmentType,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                ClientType = appointment.ClientType,
                Fees = appointment.Fees,
                AdditionalFees = appointment.AdditionalFees,
                ArrivalTime = appointment.ArrivalTime,
                EstimatedTime = appointment.EstimatedTime,
                EstimatedDuration = appointment.EstimatedDuration,
                ExactTime = appointment.ExactTime,
                EndTime = appointment.EndTime,

                FirstName = appointment.User?.Client?.FirstName ?? appointment.TemporaryClient?.FirstName,
                LastName = appointment.User?.Client?.LastName ?? appointment.TemporaryClient?.LastName,
                ContactInfo = appointment.User?.PhoneNumber ?? appointment.TemporaryClient?.ContactInfo,

                OperatorId = appointment.OperatorId,
                ProviderId = appointment.ProviderCenterService?.ProviderId,
                ServiceId = appointment.ProviderCenterService?.ServiceId,
                CenterId = appointment.ProviderCenterService?.CenterId,

                ShiftId = appointment.ShiftId,
                UserId = appointment.UserId,
                TemporaryClientId = appointment.TemporaryClientId,
                IsChecked = appointment.IsChecked
            };
            return appop;
            ;
        }
    }
}
