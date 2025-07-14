using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class OperatorServices
    {
        private readonly OperatorRepository operatorRepository;

        private readonly AccountRepository accountRepository;
        private readonly LiveQueueServices liveQueueServices;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ShiftRepository shiftRepository;
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly AppointmentServices appointmentServices;
        private readonly ProviderCenterServiceRepository providerCenterServiceRepository;
        private readonly TemperoryClientRepository temperoryClientRepository;
        private readonly UserManager<User> userManager;
        private readonly CommitData commitData;
        private readonly IHubContext<QueueHub> queueHubContext;
        private readonly IHubContext<NotificationHub> notificationHubContext;
        private readonly NotificationSignalRService _notificationSignalRService;
        private readonly NotificationServices notificationServices;

        public OperatorServices(OperatorRepository _operatorRepository,
            CommitData _commitData,
            AppointmentRepository _appointmentRepository,
            ShiftRepository _shiftRepository,
            LiveQueueRepository _liveQueueRepository,
            AppointmentServices _appointmentServices,
            IHubContext<QueueHub> _queueHubContext,
            ProviderCenterServiceRepository _providerCenterServiceRepository,
            TemperoryClientRepository _temperoryClientRepository,
            AccountRepository _accountRepository,
            UserManager<User> _userManager,
            LiveQueueServices _liveQueueServices,
            IHubContext<NotificationHub> _notificationHubContext,
                        NotificationSignalRService notificationSignalRService,
            NotificationServices _notificationServices)
        {
            shiftRepository = _shiftRepository;
            operatorRepository = _operatorRepository;
            appointmentRepository = _appointmentRepository;
            liveQueueRepository = _liveQueueRepository;
            commitData = _commitData;
            appointmentServices = _appointmentServices;
            providerCenterServiceRepository = _providerCenterServiceRepository;
            temperoryClientRepository = _temperoryClientRepository;
            accountRepository = _accountRepository;
            liveQueueServices = _liveQueueServices;
            userManager = _userManager;
            queueHubContext = _queueHubContext;
            notificationHubContext = _notificationHubContext;
            _notificationSignalRService = notificationSignalRService;
            notificationServices = _notificationServices;
        }
        public async Task<IdentityResult> CreateOperator(string userId, OperatorViewModel model)
        {
            var _operator = new Operator
            {
                OperatorId = userId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                Image = model.Image,
                CenterId = model.CenterId,
                IsDeleted = false
            };

            operatorRepository.Add(_operator);
            commitData.SaveChanges();
            return IdentityResult.Success;
        }
        public async Task<bool> DeleteOperator(string operatorId)
        {
            var selectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (selectedOperator == null)
                return false;

            operatorRepository.Delete(selectedOperator);

            var selectedUser = await userManager.FindByIdAsync(operatorId);
            if (selectedUser != null)
            {
                var result = await userManager.DeleteAsync(selectedUser);
                if (!result.Succeeded)
                {
                    return false;
                }
            }
            commitData.SaveChanges();
            return true;
        }
        public bool EditOperator(Operator _operator)
        {
            if (_operator == null)
            {
                return false;
            }
            operatorRepository.Edit(_operator);
            commitData.SaveChanges();
            return true;
        }
        public bool RestoreOperator(string operatorId)
        {
            var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator != null && SelectedOperator.IsDeleted == true)
            {
                SelectedOperator.IsDeleted = false;
                operatorRepository.Edit(SelectedOperator);
                commitData.SaveChanges();
                return true;
            }
            return false;
        }
        public bool IsExist(string operatorId)
        {
            var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

            if (SelectedOperator == null)
                return false;

            return true;
        }
        public IQueryable<Operator> GetAllOperators()
        {
            var operators = operatorRepository.GetAll().Where(t => t.IsDeleted != true);
            if (operators != null)
            {
                return operators;
            }
            return null;
        }

        public async Task<AppointmentForOperator> CreateAppointment(ReserveApointmentDTO reserveApointmentDTO)
        {
            if (reserveApointmentDTO.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
                throw new InvalidOperationException("Cannot reserve an appointment in the past.");

            var app = reserveApointmentDTO.ToAppointmentFromDTO();

            string? phoneNumber = reserveApointmentDTO.ContactInfo;
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new Exception("Phone number (ContactInfo) is required to reserve an appointment.");

            var existingUser = accountRepository
                .GetAll()
                .FirstOrDefault(u => u.PhoneNumber == phoneNumber);

            if (existingUser != null)
            {
                app.UserId = existingUser.Id;
            }
            else
            {
                var existingTempClient = temperoryClientRepository
                    .GetAll()
                    .FirstOrDefault(tc => tc.ContactInfo == phoneNumber && !tc.IsDeleted);

                if (existingTempClient != null)
                {
                    app.TemporaryClientId = existingTempClient.TempClientId;
                }
                else
                {
                    var newTempClient = new TemporaryClient
                    {
                        FirstName = string.IsNullOrWhiteSpace(reserveApointmentDTO.FirstName) ? "TempFirst" : reserveApointmentDTO.FirstName,
                        LastName = string.IsNullOrWhiteSpace(reserveApointmentDTO.LastName) ? "TempLast" : reserveApointmentDTO.LastName,
                        ContactInfo = phoneNumber,
                        TempCode = GenerateTempCode(),
                        OperatorId = reserveApointmentDTO.OperatorId,
                        IsDeleted = false
                    };
                    temperoryClientRepository.Add(newTempClient);
                    commitData.SaveChanges();

                    app.TemporaryClientId = newTempClient.TempClientId;
                }
            }

            var pcs = providerCenterServiceRepository
                .GetAll()
                .FirstOrDefault(p =>
                    p.ProviderId == reserveApointmentDTO.ProviderId &&
                    p.CenterId == reserveApointmentDTO.CenterId &&
                    p.ServiceId == reserveApointmentDTO.ServiceId);

            if (pcs == null)
                throw new Exception("Invalid provider, center, or service combination.");

            app.ProviderCenterServiceId = pcs.ProviderCenterServiceId;

            var appointmentReserved = appointmentRepository.GetList(ap => ap.ShiftId == app.ShiftId && ap.AppointmentStatus != AppointmentStatus.Cancelled).Count();

            var shiftMaxPatientsPerDay = shiftRepository.Get(shift => shift.ShiftId == app.ShiftId).Select(sh => sh.MaxPatientsPerDay).FirstOrDefault();
            if (shiftMaxPatientsPerDay <= appointmentReserved)
                throw new InvalidOperationException("Cannot reserve an appointment shift is full");

            app.EstimatedTime = appointmentServices.CalculateEstimatedTime(app.ShiftId);
            var createdAppointment = await appointmentRepository.CreateAppoinment(app);

            commitData.SaveChanges();

            var queue = appointmentServices.AssignToQueue(app.ProviderCenterServiceId, app.AppointmentDate, createdAppointment.AppointmentId);

            var Currentshift = shiftRepository.GetById(shift => shift.ShiftId == reserveApointmentDTO.ShiftId);

            if (Currentshift.ShiftType == ShiftType.OnGoing)
            {
                var newLiveQueue = new LiveQueue();
                int shiftDuration = app.ProviderCenterService.Duration;

                if (createdAppointment.AppointmentType == AppointmentType.Urgent)
                {
                    var FirstLiveQueueWaiting = liveQueueRepository.GetAllShiftLiveQueues(createdAppointment.ShiftId).OrderBy(l => l.CurrentQueuePosition).FirstOrDefault(l => l.AppointmentStatus == QueueAppointmentStatus.Waiting);
                    if (FirstLiveQueueWaiting == null)
                    {
                        FirstLiveQueueWaiting = liveQueueRepository.GetAllShiftLiveQueues(createdAppointment.ShiftId).OrderBy(l => l.CurrentQueuePosition).FirstOrDefault(l => l.AppointmentStatus == QueueAppointmentStatus.NotChecked);

                    }
                    liveQueueRepository.GetAllShiftLiveQueues(createdAppointment.ShiftId).Where(l => l.CurrentQueuePosition >= FirstLiveQueueWaiting.CurrentQueuePosition).ExecuteUpdate(p => p.SetProperty(l => l.CurrentQueuePosition, l => l.CurrentQueuePosition + 1));
                    liveQueueRepository.GetAllShiftLiveQueues(createdAppointment.ShiftId).Where(l => l.CurrentQueuePosition >= FirstLiveQueueWaiting.CurrentQueuePosition).ExecuteUpdate(p => p.SetProperty(l => l.EstimatedTime, l => l.EstimatedTime.AddMinutes(l.EstimatedDuration)));
                    newLiveQueue = new LiveQueue
                    {
                        ArrivalTime = TimeOnly.FromDateTime(DateTime.Now),
                        EstimatedTime = FirstLiveQueueWaiting.EstimatedTime,
                        EstimatedDuration = shiftDuration,
                        AppointmentStatus = QueueAppointmentStatus.Waiting,
                        Capacity = shiftMaxPatientsPerDay,
                        OperatorId = createdAppointment.OperatorId,
                        AppointmentId = createdAppointment.AppointmentId,
                        ShiftId = createdAppointment.ShiftId,
                        CurrentQueuePosition = FirstLiveQueueWaiting.CurrentQueuePosition,
                    };
                    FirstLiveQueueWaiting.EstimatedTime.AddMinutes(newLiveQueue.EstimatedDuration);
                    liveQueueRepository.Edit(newLiveQueue);
                    liveQueueRepository.Add(newLiveQueue);
                }
                else
                {
                    var lastLiveQueuePosition = liveQueueRepository.GetLiveQueueDetailsForShift(Currentshift.ShiftId).OrderByDescending(lq => lq.CurrentQueuePosition).FirstOrDefault();

                    newLiveQueue = new LiveQueue
                    {
                        ArrivalTime = TimeOnly.FromDateTime(DateTime.Now),
                        EstimatedTime = lastLiveQueuePosition.EstimatedTime.AddMinutes(lastLiveQueuePosition.EstimatedDuration),
                        EstimatedDuration = createdAppointment.ProviderCenterService.Duration,
                        AppointmentStatus = QueueAppointmentStatus.NotChecked,
                        Capacity = shiftMaxPatientsPerDay,
                        OperatorId = createdAppointment.OperatorId,
                        AppointmentId = createdAppointment.AppointmentId,
                        ShiftId = createdAppointment.ShiftId,
                        CurrentQueuePosition = lastLiveQueuePosition.CurrentQueuePosition + 1,
                    };
                    liveQueueRepository.Add(newLiveQueue);

                }
                await commitData.SaveChangesAsync();
                await UpdateQueueStatusAsync(new UpdateQueueStatusViewModel
                {
                    LiveQueueId = newLiveQueue.LiveQueueId,
                    SelectedStatus = "Waiting"
                });
            }
            return createdAppointment.ToReserveAppointmentResultDTO();
        }


        private string GenerateTempCode()
        {
            return $"TMP-{Guid.NewGuid().ToString().Substring(0, 6).ToUpper()}";
        }
        // we need the list from the algorithm
        public async Task<bool> StartShift(int ShiftId, string operatorId)
        {
            DateTime currentTime = DateTime.Now;
            TimeOnly timeNow = TimeOnly.FromDateTime(currentTime);

            Shift shift = shiftRepository.GetById(s => s.ShiftId == ShiftId);

            if (shift != null)
            {
                shift.OperatorId = operatorId;
                shift.ExactStartTime = timeNow;
                shift.ShiftType = ShiftType.OnGoing;
                shiftRepository.Edit(shift);
                if (shift?.ProviderAssignment?.Provider?.User?.Notifications != null)
                {
                    var startShiftNotification = new Notification()
                    {
                        Title = "Start Shift",
                        Message = $"Dear Dr. {shift.ProviderAssignment.Provider.FirstName} {shift.ProviderAssignment.Provider.LastName},\n" +
                              $"Your shift at {shift.ProviderAssignment.Center.CenterName} Has been started NOW at {DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")}."
                    };
                    shift.ProviderAssignment.Provider.User.Notifications.Add(startShiftNotification);

                    //await notificationHubContext.Clients.User(shift.ProviderAssignment.ProviderId).SendAsync("startShiftNotification", startShiftNotification);
                    var ProviderpaginatedNotifications = notificationServices.GetNotification(shift.ProviderAssignment.Provider.ProviderId);
                    await _notificationSignalRService.SendUpdatedNotificationList(shift.ProviderAssignment.Provider.ProviderId, ProviderpaginatedNotifications);
                }
                commitData.SaveChanges();
            }
            else
            {
                return false;
            }
            IQueryable<Appointment> appointments = appointmentRepository.GetAllShiftAppointments(ShiftId).OrderBy(app => app.EstimatedTime);
            Appointment appointmentDetails = appointmentRepository.GetAllShiftAppointments(ShiftId).OrderBy(app => app.EstimatedTime).FirstOrDefault();
            int count = 0;
            int shiftDuration = appointmentDetails.ProviderCenterService.Duration;
            //for shift
            TimeOnly ExactEstimatedTime = (TimeOnly)shift.ExactStartTime;
            foreach (var appointment in appointments)
            {
                appointment.OperatorId = operatorId;
                var livequeue = new LiveQueue
                {
                    ArrivalTime = null,
                    EstimatedTime = ExactEstimatedTime.AddMinutes(shiftDuration * count),
                    EstimatedDuration = shiftDuration,
                    AppointmentStatus = QueueAppointmentStatus.NotChecked,
                    Capacity = appointment.Shift.MaxPatientsPerDay,
                    OperatorId = appointment.OperatorId,
                    AppointmentId = appointment.AppointmentId,
                    ShiftId = appointment.ShiftId,
                    CurrentQueuePosition = ++count,
                };
                liveQueueRepository.Add(livequeue);
                var appointmentstartNotification = new Notification()
                {
                    Title = "Shift for Appointment Started",
                    Message = $"Your appointment on {appointment.AppointmentDate.ToShortDateString()} with Dr. {appointment.Shift.ProviderAssignment.Provider.FirstName} {appointment.Shift.ProviderAssignment.Provider.LastName} has been Started. We apologize for any inconvenience. Stay Tuned For any Updates!"
                };
                if (appointment.User != null)
                {

                    appointment.User.Notifications.Add(appointmentstartNotification);

                    var clientConnectionId = _notificationSignalRService.SendMessage(appointment.User.Id, new NotificationDTO
                    {
                        NotificationId = appointmentstartNotification.NotificationId,
                        Title = appointmentstartNotification.Title,
                        CreatedAt = appointmentstartNotification.CreatedAt,
                        Message = appointmentstartNotification.Message,
                        IsRead = appointmentstartNotification.IsRead
                    });
                    var ClientpaginatedNotifications = notificationServices.GetNotification(appointment.User.Id);
                    await _notificationSignalRService.SendUpdatedNotificationList(appointment.User.Id, ClientpaginatedNotifications);
                }
            }
            commitData.SaveChanges();

            return true;
        }

        public async Task<bool> EndShift(int ShiftId, string operatorId)
        {
            DateTime currentTime = DateTime.Now;
            TimeOnly timeNow = TimeOnly.FromDateTime(currentTime);

            Shift shift = shiftRepository.GetById(s => s.ShiftId == ShiftId);

            if (shift != null)
            {
                shift.ExactEndTime = timeNow;
                shift.ShiftType = ShiftType.Completed;
                shiftRepository.Edit(shift);
                if (shift?.ProviderAssignment?.Provider?.User?.Notifications != null)
                {
                    var EndShiftNotification = new Notification()
                    {
                        Title = "End Shift",
                        Message = $"Dear Dr. {shift.ProviderAssignment.Provider.FirstName} {shift.ProviderAssignment.Provider.LastName},\n" +
                              $"Your shift at {shift.ProviderAssignment.Center.CenterName} has ended at {DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")}."
                    };
                    shift.ProviderAssignment.Provider.User.Notifications.Add(EndShiftNotification);
                    commitData.SaveChanges();
                    var ProviderpaginatedNotifications = notificationServices.GetNotification(shift.ProviderAssignment.Provider.ProviderId);
                    await _notificationSignalRService.SendUpdatedNotificationList(shift.ProviderAssignment.Provider.ProviderId, ProviderpaginatedNotifications);
                }
            }
            else
            {
                return false;
            }

            IQueryable<LiveQueue> liveQueues = liveQueueRepository.GetAllShiftLiveQueues(ShiftId);

            foreach (var liveQueue in liveQueues)
            {
                liveQueueRepository.Delete(liveQueue);

            }
            commitData.SaveChanges();
            return true;
        }

        public async Task<string> UpdateQueueStatusAsync(UpdateQueueStatusViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SelectedStatus))
                return "Selected status cannot be null or empty.";

            Expression<Func<LiveQueue, bool>> predicate = lq => lq.LiveQueueId == model.LiveQueueId;

            var liveQueue = liveQueueRepository.GetById(predicate);
            if (liveQueue == null) return "Queue entry not found";

            var appointment = liveQueue.Appointment;
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now);
            int position = liveQueue.CurrentQueuePosition ?? 0;
            switch (model.SelectedStatus)
            {
                case "NotChecked":
                    if (liveQueue.AppointmentStatus != QueueAppointmentStatus.Waiting &&
                        liveQueue.AppointmentStatus != QueueAppointmentStatus.NotChecked)
                        return "Cannot transition to NotChecked from current status.";
                    liveQueue.AppointmentStatus = QueueAppointmentStatus.NotChecked;
                    appointment.IsChecked = false;
                    liveQueue.ArrivalTime = null;
                    appointment.ArrivalTime = null;
                    break;

                case "Waiting":
                    if (liveQueue.AppointmentStatus != QueueAppointmentStatus.NotChecked)
                        return "Can only transition to Waiting from NotChecked status.";
                    liveQueue.AppointmentStatus = QueueAppointmentStatus.Waiting;
                    appointment.IsChecked = true;
                    liveQueue.ArrivalTime = TimeOnly.FromDateTime(now);
                    appointment.ArrivalTime = TimeOnly.FromDateTime(now);
                    position = liveQueue.CurrentQueuePosition ?? 0;
                    await liveQueueServices.ReorderQueue(liveQueue.ShiftId);
                    break;

                case "InProgress":
                    if (liveQueue.AppointmentStatus != QueueAppointmentStatus.Waiting)
                        return "Can only transition to InProgress from Waiting status.";
                    if (!liveQueue.ArrivalTime.HasValue)
                        return "Arrival time must be set before transitioning to InProgress.";
                    liveQueue.AppointmentStatus = QueueAppointmentStatus.InProgress;
                    appointment.IsChecked = true;
                    appointment.ExactTime = TimeOnly.FromDateTime(now);
                    break;

                case "Completed":
                    if (liveQueue.AppointmentStatus != QueueAppointmentStatus.InProgress)
                        return "Can only transition to Completed from InProgress status.";
                    if (appointment.ExactTime == TimeOnly.MinValue)
                        return "Exact time must be set before transitioning to Completed.";
                    liveQueue.AppointmentStatus = QueueAppointmentStatus.Completed;
                    appointment.IsChecked = true;
                    appointment.EndTime = TimeOnly.FromDateTime(now);
                    appointment.AdditionalFees = model.AdditionalFees ?? 0m;
                    appointment.AppointmentStatus = AppointmentStatus.Completed;
                    position = liveQueue.CurrentQueuePosition ?? 0;
                    await liveQueueServices.ReorderQueue(liveQueue.ShiftId);

                    break;

                default:
                    return "Invalid status selected";
            }

            appointment.UpdatedAt = now;
            commitData.SaveChanges();



            await queueHubContext.Clients.Group($"shift_{appointment.ShiftId}").SendAsync("ReceiveQueueStatusUpdate", model.LiveQueueId, model.SelectedStatus);
            await liveQueueServices.NotifyShiftQueueUpdate(appointment.ShiftId);
            return "Queue status updated successfully";
        }

        public List<Operator> GetOperatorsByCenterId(int centerId)
        {
            return operatorRepository.GetAll().Where(o => o.CenterId == centerId).ToList();
        }
        public PaginationViewModel<OperatorViewModel> Search(string searchText = "", int pageNumber = 1,
                                                    int pageSize = 5)
        {
            return operatorRepository.Search(searchText, pageNumber, pageSize);
        }
        public Operator GetOperatorsById(string opertorId)
        {
            return operatorRepository.GetById(o => o.OperatorId == opertorId);
        }
    }
}
