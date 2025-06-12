using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly ShiftRepository shiftRepository;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;
        private readonly PaymentRepository paymentRepository;

        public AppointmentServices(CommitData _commitData,PaymentRepository _paymentRepository,PaymentServices _paymentServices, AppointmentRepository _appointmentRepository,ProviderCenterServiceRepository _providerCenterServiceRepository, ShiftRepository _shiftRepository)
        {
            commitData = _commitData;
            paymentRepository = _paymentRepository;
            paymentServices = _paymentServices;
            appointmentRepository = _appointmentRepository;
            shiftRepository = _shiftRepository;
            providerCenterServiceRepository = _providerCenterServiceRepository;
        }


        public Appointment ReserveAppointment(ReserveApointmentDTO reserveApointmentDTO)
        {
            if (reserveApointmentDTO.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
                throw new InvalidOperationException("Cannot reserve an appointment in the past.");

            var app = reserveApointmentDTO.reserveApointmentDTOToAppointment();

            var pcs = providerCenterServiceRepository
                .GetAll()
                .FirstOrDefault
                (   p =>
                    p.ProviderId == reserveApointmentDTO.ProviderId &&
                    p.CenterId == reserveApointmentDTO.CenterId &&
                    p.ServiceId == reserveApointmentDTO.ServiceId
                );

            if (pcs == null)
                throw new Exception("Invalid provider, center, or service combination.");

            app.ProviderCenterServiceId = pcs.ProviderCenterServiceId;
           
            Appointment createdAppointment = appointmentRepository.CreateAppoinment(app);

            createdAppointment.EstimatedTime = CalculateEstimatedTime(app.ShiftId);

            commitData.SaveChanges();

            var queue = AssignToQueue(app.ProviderCenterServiceId, app.AppointmentDate, createdAppointment.AppointmentId);

            var queuedAppointment = queue.FirstOrDefault(a => a.AppointmentId == createdAppointment.AppointmentId);
            if (queuedAppointment != null)
            {
                createdAppointment.EstimatedTime = queuedAppointment.EstimatedTime;
            }

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

        public List<AppointmentForClientProfileDTO> GetAppointmentsByUserId(string userId)
        {
            var appointments = appointmentRepository.GetAppointmentsByClientId(userId).Select(p=>p.AppointmentToAppointmentForClientProfileDTO());
            return appointments.ToList();
        }


        public AppointmentDTO GetLastAppointment(string userId)
        {
            var appointments = appointmentRepository.GetAppointmentsByClientId(userId)
                                                    .OrderByDescending(a => a.AppointmentDate)
                                                    .FirstOrDefault();

            return appointments?.AppointmentToAppointmentDTO();
        }

        public List<AppointmentForClientProfileDTO> GetUpcomingAppointments(string userId)
        {
            var upcoming = appointmentRepository.GetAppointmentsByClientId(userId)
                           .Where(a=>a.AppointmentDate>=DateOnly.FromDateTime(DateTime.Now)).Select(a=>a.AppointmentToAppointmentForClientProfileDTO());
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
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error canceling appointment {appointment.AppointmentId}: {ex.Message}");
                    }
                }
            }
        }

        public List<Appointment> AssignToQueue(int providerCenterServiceId, DateOnly shiftDate, int? newAppointmentId)
        {
            var confirmedAppointments = appointmentRepository.GetAll()
                .Where(a => (a.AppointmentStatus == AppointmentStatus.Confirmed ||
                             a.AppointmentId == newAppointmentId) &&
                            a.AppointmentDate == shiftDate &&
                            a.ProviderCenterServiceId == providerCenterServiceId)
                .ToList();

            var queue = new List<Appointment>();
            int insertIndex = 0;


            var newUrgent = confirmedAppointments
                .FirstOrDefault(a => a.AppointmentId == newAppointmentId && a.ClientType == ClientType.Urgent && a.AppointmentType == AppointmentType.Urgent);

            if (newUrgent != null)
            {

                TimeOnly insertTime;
                var existingAppointments = confirmedAppointments
                    .Where(a => a.AppointmentId != newAppointmentId)
                    .ToList();

                if (existingAppointments.Any())
                {
                    insertTime = existingAppointments.Min(a => a.EstimatedTime);
                }
                else
                {
                    var now = DateTime.Now;
                    insertTime = new TimeOnly(now.Hour, now.Minute);
                }

                newUrgent.EstimatedTime = insertTime;
                queue.Insert(insertIndex, newUrgent);
                insertIndex++;


                var remainingAppointments = confirmedAppointments
                    .Where(a => a.AppointmentId != newAppointmentId &&
                                (a.ClientType == ClientType.Normal ||
                                 a.ClientType == ClientType.Consultation)
                                )
                    .OrderBy(a => a.EstimatedTime)
                    .ToList();

                foreach (var app in remainingAppointments)
                {
                    var previousAppointment = queue[insertIndex - 1];
                    insertTime = previousAppointment.EstimatedTime.Add(TimeSpan.FromMinutes(previousAppointment.EstimatedDuration));
                    app.EstimatedTime = insertTime;
                    queue.Insert(insertIndex, app);
                    insertIndex++;
                }
            }
            else
            {

                var regularAppointments = confirmedAppointments
                    .Where(a => a.ClientType == ClientType.Normal ||
                                a.ClientType == ClientType.Consultation
                                )
                    .OrderBy(a => a.EstimatedTime)
                    .ToList();

                TimeOnly insertTime = regularAppointments.Any() ? regularAppointments.Min(a => a.EstimatedTime) : new TimeOnly(DateTime.Now.Hour, DateTime.Now.Minute);
                foreach (var app in regularAppointments)
                {
                    if (app.AppointmentId == newAppointmentId || app.EstimatedTime == TimeOnly.MinValue)
                    {
                        app.EstimatedTime = insertTime;
                    }
                    queue.Add(app);
                    insertTime = app.EstimatedTime.Add(TimeSpan.FromMinutes(app.EstimatedDuration));
                }
            }


            foreach (var app in queue)
            {
                var dbApp = appointmentRepository.GetById(a => a.AppointmentId == app.AppointmentId);
                if (dbApp != null && dbApp.EstimatedTime != app.EstimatedTime)
                {
                    dbApp.EstimatedTime = app.EstimatedTime;
                    appointmentRepository.Edit(dbApp);
                }
            }

            commitData.SaveChanges();

            return queue;
        }

        public TimeOnly CalculateEstimatedTime(int shiftId)
        {
            var appointments = appointmentRepository.GetAll().Where(a=>a.ShiftId==shiftId);
            int TotalDuration = 0;
            foreach (var appointment in appointments)
            {
                TotalDuration += appointment.ProviderCenterService.Duration;
            }
            var shift = shiftRepository.GetById(s => s.ShiftId == shiftId);
            return shift.StartTime.AddMinutes(TotalDuration);
        }
    }
}
