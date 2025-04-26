using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Services
{
    public class AppointmentServices
    {
        private readonly CommitData commitData;
        private readonly PaymentServices paymentServices;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;
        private readonly PaymentRepository paymentRepository;

        public AppointmentServices(CommitData _commitData,PaymentRepository _paymentRepository,PaymentServices _paymentServices, AppointmentRepository _appointmentRepository,ProviderCenterServiceRepository _providerCenterServiceRepository)
        {
            commitData = _commitData;
            paymentRepository = _paymentRepository;
            paymentServices = _paymentServices;
            appointmentRepository = _appointmentRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
        }


        public async Task<Appointment> ReserveAppointment(AppointmentDTO appointmentDTO, string stripeToken, decimal amount, string clientId)
        {

            // Validate appointment details
            if (appointmentDTO.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
                throw new InvalidOperationException("Cannot reserve an appointment in the past.");

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

            Appointment createdAppointment;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    createdAppointment = appointmentRepository.CreateAppoinment(app);
                    commitData.SaveChanges();
                    await paymentServices.ProcessPayment(stripeToken, amount, clientId, createdAppointment.AppointmentId);

                    transaction.Complete();
                }
                catch (Exception) {

                    throw;
                }
            }

            return createdAppointment;
        }

        public async Task CancelAppointment(int appointmentId)
        {
            var appointment = appointmentRepository.GetById(a => a.AppointmentId == appointmentId);
            if (appointment == null)
                throw new Exception("Appointment not found.");

            // Find the associated payment
            var payment = paymentRepository.GetById(p => p.AppointmentId == appointmentId);
            if (payment != null && payment.RefundStatus != "refunded")
            {
                await paymentServices.RefundPayment(payment.TransactionId, appointmentId);
            }

            appointmentRepository.Delete(appointment);
            commitData.SaveChanges();
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
            var upcoming = appointmentRepository.GetAppointmentsByClientId(userId)
                           .Where(a=>a.AppointmentDate>=DateOnly.FromDateTime(DateTime.Now)).Select(a=>a.AppointmentToAppointmentDTO());
            return upcoming.ToList();
        }

    }
}
