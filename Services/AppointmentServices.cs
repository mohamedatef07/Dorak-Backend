using Data;
using Dorak.Models;
using Dorak.ViewModels;
using Models.Enums;
using Repositories;
using Stripe;
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


        public Appointment ReserveAppointment(AppointmentDTO appointmentDTO)
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
            createdAppointment = appointmentRepository.CreateAppoinment(app);
            commitData.SaveChanges();



            return createdAppointment;
        }

        public async Task<Charge> ProcessPayment(string stripeToken, decimal amount, string clientId, int appointmentId)
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {

                    var res = await paymentServices.ProcessPayment(stripeToken, amount, clientId, appointmentId);
                    var app = appointmentRepository.GetById(a=>a.AppointmentId==appointmentId);
                    app.AppointmentStatus = AppointmentStatus.Confirmed;
                    
                    commitData.SaveChanges();



                    transaction.Complete();
                    return res;

                }

            }
            catch (StripeException ex)
            {
                throw new Exception($"Payment failed: {ex.Message}");
            }
            catch(Exception ex) {
                throw new Exception($"{ex.Message}");

            }

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

        public Appointment GetAppointmentById(int AppointmentId)
        {
            return appointmentRepository.GetById(a=>a.AppointmentId==AppointmentId);
        }


        public async Task CancelUnpaidAppointments()
        {
            // Get all appointments scheduled for two days from now
            var upcomingAppointments = appointmentRepository.GetUpcomingAppointments()
                .Where(a => a.AppointmentDate == DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
                .ToList();

            foreach (var appointment in upcomingAppointments)
            {
                var payment = paymentRepository.GetById(p => p.AppointmentId == appointment.AppointmentId);

                if (payment == null || payment.PaymentStatus != "succeeded") 
                {
                    try
                    {
                        // Proceed to cancel the appointment
                        await CancelAppointment(appointment.AppointmentId);
                        Console.WriteLine($"Appointment {appointment.AppointmentId} canceled due to non-payment.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error canceling appointment {appointment.AppointmentId}: {ex.Message}");
                    }
                }
            }
        }

    }
}
