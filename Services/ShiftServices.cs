using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Enums;
using Repositories;

namespace Services
{
    public class ShiftServices
    {
        private readonly ShiftRepository shiftRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly CenterServices centerServices;
        private readonly WalletRepository walletRepository;
        private readonly CommitData commitData;
        private readonly IHubContext<ShiftListHub> shiftListHubContext;
        private readonly IHubContext<NotificationHub> notificationHubContext;
        private readonly NotificationServices notificationServices;

        public ShiftServices(ShiftRepository _shiftRepository, AppointmentRepository _appointmentRepository, LiveQueueRepository _liveQueueRepository, CommitData _commitData, CenterServices _centerServices, WalletRepository _walletRepository, IHubContext<ShiftListHub> _shiftListHubContext, IHubContext<NotificationHub> _notificationHubContext, NotificationServices notificationServices)
        {
            shiftRepository = _shiftRepository;
            appointmentRepository = _appointmentRepository;
            liveQueueRepository = _liveQueueRepository;
            commitData = _commitData;
            centerServices = _centerServices;
            shiftListHubContext = _shiftListHubContext;
            notificationHubContext = _notificationHubContext;
            this.notificationServices = notificationServices;
            walletRepository = _walletRepository;
        }
        public Shift GetShiftById(int shiftId)
        {
            return shiftRepository.GetShiftById(shiftId);
        }
        public IQueryable<ShiftDTO> GetShiftsWithDateAndCenterId(DateOnly _shiftDate, int centerId)
        {
            var shifts = shiftRepository.GetShiftsWithDateAndCenterId(_shiftDate, centerId);
            return shifts.Select(shift => shift.ShiftToShiftVM());
        }

        public IQueryable<ShiftDTO> GetShiftsWithDateAndProvider(DateOnly _shiftDate, string providerId)
        {
            var shifts = shiftRepository.GetShiftsWithDateAndProvider(_shiftDate, providerId);
            return shifts.Select(shift => shift.ShiftToShiftVM());
        }

        public IQueryable<AppointmentDTO> GetAppointmentsByShiftId(int ShiftId)
        {
            var appointments = appointmentRepository.GetAllShiftAppointments(ShiftId);
            return appointments.Select(app => app.AppointmentToAppointmentDTO());
        }

        public IQueryable<Appointment> LiveShiftAppointments()
        {
            var shifts = shiftRepository.LiveQueueShift().ToList();
            foreach (var shift in shifts)
            {
                var appointments = appointmentRepository.GetAllShiftAppointments(shift.ShiftId);
                foreach (var appointment in appointments)
                {

                    liveQueueRepository.Add(new LiveQueue
                    {
                        ShiftId = appointment.ShiftId,
                        OperatorId = appointment.OperatorId,
                        AppointmentId = appointment.AppointmentId,
                        ArrivalTime = null,
                        EstimatedTime = appointment.EstimatedTime,
                        CurrentQueuePosition = liveQueueRepository.GetCurrentPostion(appointment),
                        EstimatedDuration = appointment.ProviderCenterService.Duration,
                    });
                }

            }
            return null;
        }
        public List<GetAllCenterShiftsDTO> GetAllCenterShifts(Center center)
        {
            if (center?.ProviderAssignments == null || !center.ProviderAssignments.Any())
            {
                return new List<GetAllCenterShiftsDTO>();
            }
            var proivderAssignments = center.ProviderAssignments;

            var shifts = proivderAssignments.SelectMany(
                pa =>
                     pa.Shifts.Where(sh => sh.ShiftType != ShiftType.Cancelled).Select(shift => new GetAllCenterShiftsDTO
                     {
                         ProviderName = $"{pa.Provider.FirstName} {pa.Provider.LastName}",
                         ShiftId = shift.ShiftId,
                         ShiftType = shift.ShiftType,
                         ShiftDate = shift.ShiftDate,
                         StartTime = shift.StartTime,
                         EndTime = shift.EndTime,
                     })
                ).ToList();
            return shifts;
        }

        public List<GetAllCenterShiftAndServicesDTO> GetAllCenterShiftsAndServices(Center center)
        {
            if (center?.ProviderAssignments == null || !center.ProviderAssignments.Any())
            {
                return new List<GetAllCenterShiftAndServicesDTO>();
            }
            var proivderAssignments = center.ProviderAssignments;
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);

            var shifts = proivderAssignments.SelectMany(
                pa =>
                     pa.Shifts.Where(sh => sh.ShiftType != ShiftType.Cancelled && sh.ShiftDate >= dateNow && sh.ShiftDate <= dateNow.AddDays(30)).Select(shift => new GetAllCenterShiftAndServicesDTO
                     {
                         ProviderName = $"{pa.Provider.FirstName} {pa.Provider.LastName}",
                         ProviderId = pa.Provider.ProviderId,
                         ShiftId = shift.ShiftId,
                         ShiftDate = shift.ShiftDate,
                         StartTime = shift.StartTime,
                         EndTime = shift.EndTime,
                         Services = pa.Provider.ProviderCenterServices
                             .Where(pcs => pcs.CenterId == center.CenterId)
                             .Select(pcs => new ServicesDTO
                             {
                                 ServiceId = pcs.Service.ServiceId,
                                 ServiceName = pcs.Service.ServiceName,
                                 BasePrice = pcs.Service.BasePrice
                             }).ToList()
                     })
                            ).ToList();

            return shifts;
        }


        public async Task<bool> ShiftCancelation(Shift shift, int centerId)
        {
            if (shift == null)
            {
                return false;
            }
            if (shift.ShiftType == ShiftType.Cancelled)
            {
                return false;
            }
            shift.ShiftType = ShiftType.Cancelled;
            var shiftCancelNotification = new Notification()
            {
                Title = "Shift Cancellation",
                Message = $"Dear Dr. {shift.ProviderAssignment.Provider.FirstName} {shift.ProviderAssignment.Provider.LastName}," +
                $" Your shift scheduled for {shift.ShiftDate.ToShortDateString()} from {shift.StartTime.ToShortTimeString()} to {shift.EndTime.ToShortTimeString()} at {shift.ProviderAssignment.Center.CenterName} Center has been canceled.\n" +
                " Further information will be provided shortly."
            };
            shift.ProviderAssignment.Provider.User.Notifications.Add(shiftCancelNotification);
            commitData.SaveChanges();
            var providerConnectionId = notificationServices.GetConnectionId(shift.ProviderAssignment.Provider.ProviderId);
            if (!string.IsNullOrEmpty(providerConnectionId))
            {
                await notificationHubContext.Clients.Client(providerConnectionId).SendAsync("shiftCancellationNotification", shiftCancelNotification);

                // Send the updated notifications list to the provider
                var updatedNotificationsAfterCancellation = shift.ProviderAssignment.Provider.User.Notifications.OrderBy(no => no.CreatedAt).ToList();
                await notificationHubContext.Clients.Client(providerConnectionId).SendAsync("updatedNotifications", updatedNotificationsAfterCancellation);
            }

            if (shift.Appointments != null && shift.Appointments.Any())
            {
                foreach (var appointment in shift.Appointments.ToList())
                {
                    if (appointment.AppointmentStatus == AppointmentStatus.Confirmed)
                    {
                        var wallet = walletRepository.GetWalletByUserId(appointment.User.Id);
                        if (wallet == null)
                        {
                            walletRepository.Add(new Wallet { ClientId = appointment.User.Id, Balance = appointment.Fees });
                            commitData.SaveChanges();
                        }
                        else
                        {
                            appointment.User.Wallet.Balance += appointment.Fees;
                            commitData.SaveChanges();
                        }
                    }
                    appointment.AppointmentStatus = AppointmentStatus.Cancelled;
                    var appointmentCancelationNotification = new Notification()
                    {
                        Title = "Appointment Cancellation",
                        Message = $"Your appointment on {appointment.AppointmentDate.ToShortDateString()} with Dr. {appointment.Shift.ProviderAssignment.Provider.FirstName} {appointment.Shift.ProviderAssignment.Provider.LastName} has been canceled. We apologize for any inconvenience. Please contact us or reschedule your appointment at your earliest convenience."
                    };
                    if (appointment.User != null)
                    {

                        appointment.User.Notifications.Add(appointmentCancelationNotification);
                        commitData.SaveChanges();

                        var clientConnectionId = notificationServices.GetConnectionId(appointment.User.Id);
                        if (!string.IsNullOrEmpty(clientConnectionId))
                        {
                            await notificationHubContext.Clients.Client(clientConnectionId).SendAsync("appointmentCancellationNotification", appointmentCancelationNotification);

                            // Send the updated notifications list to the client
                            var updatedNotificationsAfterAppointmentCancellation = appointment.User.Notifications.OrderBy(no => no.CreatedAt).ToList();
                            await notificationHubContext.Clients.Client(clientConnectionId).SendAsync("updatedNotifications", updatedNotificationsAfterAppointmentCancellation);
                        }
                    }
                }
                shiftRepository.Edit(shift);
                commitData.SaveChanges();

                var center = centerServices.GetCenterById(centerId);
                var updatedShiftList = GetAllCenterShifts(center);
                var OperatorCconnectionId = ShiftListHub.GetConnectionId(shift.Operator.OperatorId);

                if (OperatorCconnectionId != null)
                {
                    await shiftListHubContext.Clients.Client(OperatorCconnectionId).SendAsync("updateShiftsList", updatedShiftList);
                }

                return true;
            }
            return false;
        }
    }


}
