using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AppointmentServices
    {
        private readonly CommitData commitData;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;

        public AppointmentServices(CommitData _commitData, AppointmentRepository _appointmentRepository,ProviderCenterServiceRepository _providerCenterServiceRepository)
        {
            commitData = _commitData;
            appointmentRepository = _appointmentRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
        }


        public Appointment ReserveAppointment(AppointmentDTO appointmentDTO)
        {
            var app = appointmentDTO.AppointmentDTOToAppointment();
            var pcs = providerCenterServiceRepository
                .GetAll()
                .FirstOrDefault(p =>
                    p.ProviderId == appointmentDTO.ProviderId &&
                    p.CenterId == appointmentDTO.CenterId &&
                    p.ServiceId == appointmentDTO.ServiceId);

            if (pcs == null)
                throw new Exception("Invalid provider, center, or service combination.");

            app.ProviderCenterServiceId = pcs.ProviderCenterServiceId;

            var createdAppointment = appointmentRepository.CreateAppoinment(app);
            return createdAppointment;
        }

        public List<Appointment> GetAppointmentsByUserId(string userId)
        {
            var appointments = appointmentRepository.GetAppointmentsByClientId(userId).ToList();
            return appointments;
        }


        public AppointmentDTO GetLastAppointment(string userId)
        {
            var appointments = appointmentRepository.GetAppointmentsByClientId(userId)
                                                    .OrderByDescending(a => a.AppointmentDate)
                                                    .FirstOrDefault();

            return appointments?.AppointmentToAppointmentDTO();
        }

        public List<AppointmentDTO> GetUpcomingAppointments(string userId)
        {
            var upcoming = appointmentRepository.GetAppointmentsByClientId(userId);
            return upcoming.Select(a => a.AppointmentToAppointmentDTO()).ToList();
        }

    }
}
