using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Models.Enums;
using Repositories;
using System.Linq.Expressions;

namespace Services
{
    public class OperatorServices
    {
        private readonly OperatorRepository operatorRepository;
        private readonly ClientRepository clientRepository;
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


        public OperatorServices(OperatorRepository _operatorRepository, CommitData _commitData, AppointmentRepository _appointmentRepository, ClientRepository _clientRepository, ShiftRepository _shiftRepository, LiveQueueRepository _liveQueueRepository, AppointmentServices _appointmentServices, IHubContext<QueueHub> _queueHubContext, ProviderCenterServiceRepository _providerCenterServiceRepository, TemperoryClientRepository _temperoryClientRepository, AccountRepository _accountRepository, UserManager<User> _userManager, LiveQueueServices _liveQueueServices, IHubContext<NotificationHub> _notificationHubContext)
        {
            shiftRepository = _shiftRepository;
            operatorRepository = _operatorRepository;
            clientRepository = _clientRepository;
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


        //public bool DeleteOperator(string operatorId)
        //{
        //    var SelectedOperator = operatorRepository.GetById(o => o.OperatorId == operatorId);

        //    if (SelectedOperator != null)
        //    {
        //        SelectedOperator.IsDeleted = true;
        //        operatorRepository.Edit(SelectedOperator);
        //        commitData.SaveChanges();
        //        return true;
        //    }
        //    return false;
        //}
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

            var createdAppointment = await appointmentRepository.CreateAppoinment(app);
            createdAppointment.EstimatedTime = appointmentServices.CalculateEstimatedTime(app.ShiftId);

            Console.WriteLine(" Saving to DB...");
            commitData.SaveChanges();
            Console.WriteLine(" Done saving to DB");
            var queue = appointmentServices.AssignToQueue(app.ProviderCenterServiceId, app.AppointmentDate, createdAppointment.AppointmentId);

            var queuedAppointment = queue.FirstOrDefault(a => a.AppointmentId == createdAppointment.AppointmentId);
            if (queuedAppointment != null)
            {
                createdAppointment.EstimatedTime = queuedAppointment.EstimatedTime;
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
                commitData.SaveChanges();
                var startShiftNotification = new Notification()
                {
                    Title = "Start Shift",
                    Message = $"Dear Dr. {shift.ProviderAssignment.Provider.FirstName} {shift.ProviderAssignment.Provider.LastName},\n" +
                              $"Your shift at {shift.ProviderAssignment.Center.CenterName} has officially begun."
                };
                shift.ProviderAssignment.Provider.User.Notifications.Add(startShiftNotification);
                commitData.SaveChanges();
                await notificationHubContext.Clients.User(shift.ProviderAssignment.ProviderId).SendAsync("startShiftNotification", startShiftNotification);
            }
            else
            {
                return false;
            }
            IQueryable<Appointment> appointments = appointmentRepository.GetAllShiftAppointments(ShiftId).OrderBy(app => app.EstimatedTime);
            int count = 0;
            foreach (var appointment in appointments)
            {
                appointment.OperatorId = operatorId;
                var livequeue = new LiveQueue
                {
                    ArrivalTime = null,
                    EstimatedTime = appointment.EstimatedTime,
                    EstimatedDuration = appointment.ProviderCenterService.Duration,
                    AppointmentStatus = QueueAppointmentStatus.NotChecked,
                    Capacity = appointment.Shift.MaxPatientsPerDay,
                    OperatorId = appointment.OperatorId,
                    AppointmentId = appointment.AppointmentId,
                    ShiftId = appointment.ShiftId,
                    CurrentQueuePosition = ++count,
                };
                liveQueueRepository.Add(livequeue);
            }
            commitData.SaveChanges();
            return true;
        }

        public bool EndShift(int ShiftId, string operatorId)
        {
            DateTime currentTime = DateTime.Now;
            TimeOnly timeNow = TimeOnly.FromDateTime(currentTime);

            Shift shift = shiftRepository.GetById(s => s.ShiftId == ShiftId);

            if (shift != null)
            {
                shift.ExactEndTime = timeNow;
                shift.ShiftType = ShiftType.Completed;
                shiftRepository.Edit(shift);
                commitData.SaveChanges();
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
                    int position = liveQueue.CurrentQueuePosition ?? 0;
                    liveQueueServices.editTurn(liveQueue.ShiftId, position);
                    break;

                default:
                    return "Invalid status selected";
            }

            appointment.UpdatedAt = now;
            commitData.SaveChanges();



            await queueHubContext.Clients.All.SendAsync("ReceiveQueueStatusUpdate", model.LiveQueueId, model.SelectedStatus);
            await liveQueueServices.NotifyShiftQueueUpdate(appointment.ShiftId);
            return "Queue status updated successfully";
        }

        public List<Operator> GetOperatorsByCenterId(int centerId)
        {
            return operatorRepository.GetAll().Where(o => o.CenterId == centerId).ToList();
        }
    }
}
