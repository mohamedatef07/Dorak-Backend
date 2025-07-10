using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Enums;
using Repositories;
using System.Data.Entity;
using System.Net.Sockets;

namespace Services
{
    public class LiveQueueServices
    {
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly ShiftRepository shiftRepository;
        private readonly CommitData commitData;
        private readonly IHubContext<QueueHub> hubContext;
        private readonly NotificationSignalRService notificationSignalRService;
        private readonly NotificationServices notificationServices;

        public LiveQueueServices(LiveQueueRepository _liveQueueRepository,
            AppointmentRepository appointmentRepository,
            ProviderServices _providerServices,
            ClientRepository _clientRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            OperatorRepository _operatorRepository,
            ShiftRepository _shiftRepository,
            IHubContext<QueueHub> hubContext,
            CommitData _commitData,
            NotificationSignalRService _notificationSignalRService,
            NotificationServices _notificationServices)
        {
            liveQueueRepository = _liveQueueRepository;
            this.appointmentRepository = appointmentRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            shiftRepository = _shiftRepository;
            this.hubContext = hubContext;
            commitData = _commitData;
            notificationSignalRService = _notificationSignalRService;
            notificationServices = _notificationServices;
        }

        //from provider side
        public List<GetQueueEntriesDTO> GetQueueEntries(Provider provider)
        {
            List<ProviderAssignment> providerAssignments = providerAssignmentRepository.GetCurrentAssignmentsForProvider(provider.ProviderId);
            List<Shift> shifts = providerAssignments.SelectMany(pa => pa.Shifts.Where(sh => sh.ProviderAssignmentId == pa.AssignmentId && sh.ShiftType == ShiftType.OnGoing)).ToList();
            List<LiveQueue> liveQueues = shifts.SelectMany(sh => sh.LiveQueues.Where(lq => lq.ShiftId == sh.ShiftId)).ToList();
            List<GetQueueEntriesDTO> result = new List<GetQueueEntriesDTO>();
            foreach (var liveQueue in liveQueues)
            {
                var appointment = liveQueue.Appointment;
                if (appointment.User == null)
                {
                    result.Add(new GetQueueEntriesDTO
                    {
                        FullName = $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
                        ArrivalTime = liveQueue.ArrivalTime,
                        AppointmentDate = appointment.AppointmentDate,
                        ClientType = appointment.ClientType,
                        Status = liveQueue.AppointmentStatus,
                        PhoneNumber = appointment.TemporaryClient.ContactInfo,
                        CurrentQueuePosition = liveQueue.CurrentQueuePosition

                    });
                }
                else
                {
                    result.Add(new GetQueueEntriesDTO
                    {
                        FullName = $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}",
                        ArrivalTime = liveQueue.ArrivalTime,
                        AppointmentDate = appointment.AppointmentDate,
                        ClientType = appointment.ClientType,
                        Status = liveQueue.AppointmentStatus,
                        PhoneNumber = appointment.User.PhoneNumber,
                        CurrentQueuePosition = liveQueue.CurrentQueuePosition
                    });
                }
            }
            return result;
        }
        //from owner side
        public PaginationViewModel<ProviderLiveQueueViewModel> GetLiveQueuesForProvider(int centerId, int shiftId, int pageNumber = 1, int pageSize = 16)
        {
            Provider shiftProvider = shiftRepository.GetShiftById(shiftId).ProviderAssignment.Provider;
            var providerAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.CenterId == centerId && !pa.IsDeleted)
                .ToList();

            if (!providerAssignments.Any())
            {
                return new PaginationViewModel<ProviderLiveQueueViewModel>
                {
                    Data = new List<ProviderLiveQueueViewModel>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Total = 0
                };
            }


            List<Shift> shifts = providerAssignments
                .SelectMany(pa => pa.Shifts
                    .Where(sh => sh.ProviderAssignmentId == pa.AssignmentId && sh.ShiftId == shiftId && sh.ShiftType == ShiftType.OnGoing && !sh.IsDeleted))
                .ToList();

            if (!shifts.Any())
            {
                return new PaginationViewModel<ProviderLiveQueueViewModel>
                {
                    Data = new List<ProviderLiveQueueViewModel>(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Total = 0
                };
            }

            var liveQueues = shifts.SelectMany(sh => sh.LiveQueues
                .Where(lq => lq.ShiftId == sh.ShiftId))
                .OrderBy(lq => lq.CurrentQueuePosition)
                .ToList();

            var total = liveQueues.Count;

            var paginatedLiveQueues = liveQueues
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            List<ProviderLiveQueueViewModel> result = new List<ProviderLiveQueueViewModel>();
            foreach (var liveQueue in paginatedLiveQueues)
            {
                var appointment = liveQueue.Appointment;
                if (appointment.User == null)
                {
                    result.Add(new ProviderLiveQueueViewModel
                    {
                        LiveQueueId = liveQueue.LiveQueueId,
                        ClientFullName = $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
                        ArrivalTime = liveQueue.ArrivalTime,
                        EstimatedTime = liveQueue.EstimatedTime,
                        ClientType = appointment.ClientType,
                        Status = liveQueue.AppointmentStatus,
                        PhoneNumber = appointment.TemporaryClient.ContactInfo,
                        CurrentQueuePosition = liveQueue.CurrentQueuePosition,
                        AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>(),
                        ProviderName = shiftProvider.FirstName + " " + shiftProvider.LastName

                    });
                }
                else
                {
                    result.Add(new ProviderLiveQueueViewModel
                    {
                        LiveQueueId = liveQueue.LiveQueueId,
                        ClientFullName = $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}",
                        ArrivalTime = liveQueue.ArrivalTime,
                        EstimatedTime = liveQueue.EstimatedTime,
                        ClientType = appointment.ClientType,
                        Status = liveQueue.AppointmentStatus,
                        PhoneNumber = appointment.User.PhoneNumber,
                        CurrentQueuePosition = liveQueue.CurrentQueuePosition,
                        AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>(),
                        ProviderName = shiftProvider.FirstName + " " + shiftProvider.LastName
                    });
                }
            }

            return new PaginationViewModel<ProviderLiveQueueViewModel>
            {
                Data = result,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Total = total
            };
        }


        public List<ClientLiveQueueDTO> GetQueueEntryByAppointmentId(int appointmentId)
        {
            var app = appointmentRepository.GetById(a => a.AppointmentId == appointmentId);
            if (app == null)
            {
                return null;
            }
            var liveQueues = liveQueueRepository.GetLiveQueueDetailsForShift(app.ShiftId);

            var LqAppointment = liveQueueRepository.GetById(app => app.AppointmentId == appointmentId);
            if (LqAppointment == null)
            {
                return null;
            }
            var result = liveQueues.Where(lq => lq.AppointmentStatus != QueueAppointmentStatus.Completed).Select(lq => new ClientLiveQueueDTO
            {

                ArrivalTime = lq.ArrivalTime,
                AppointmentDate = lq.Appointment.AppointmentDate,
                Type = lq.Appointment.ClientType,
                Status = lq.AppointmentStatus,
                AppointmentId = appointmentId,
                CurrentQueuePosition = lq.CurrentQueuePosition,
                IsCurrentClient = lq.Appointment.AppointmentId == appointmentId
            }).ToList();

            return result;
        }

        public List<ClientLiveQueueDTO> GetLiveQueueForShift(int shiftId, string? userId)
        {
            var liveQueues = liveQueueRepository.GetLiveQueueDetailsForShift(shiftId);
            var result = new List<ClientLiveQueueDTO>();

            foreach (var liveQueue in liveQueues)
            {
                var appointment = liveQueue.Appointment;

                var dto = new ClientLiveQueueDTO
                {
                    ArrivalTime = liveQueue.ArrivalTime,
                    AppointmentDate = appointment.AppointmentDate,
                    Type = appointment.ClientType,
                    Status = liveQueue.AppointmentStatus,

                    CurrentQueuePosition = liveQueue.CurrentQueuePosition,

                    IsCurrentClient = appointment.UserId == userId
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task NotifyShiftQueueUpdate(int shiftId)
        {
            var liveQueueList = liveQueueRepository
                .GetLiveQueueDetailsForShift(shiftId).Where(lq => lq.AppointmentStatus != QueueAppointmentStatus.Completed)//we dont get the completed lq from db 
                .Select(lq => new ClientLiveQueueDTO
                {

                    ArrivalTime = lq.ArrivalTime,
                    AppointmentDate = lq.Appointment.AppointmentDate,
                    Type = lq.Appointment.ClientType,
                    Status = lq.AppointmentStatus,
                    AppointmentId = lq.AppointmentId,
                    CurrentQueuePosition = lq.CurrentQueuePosition,
                    IsCurrentClient = false // سيتم تحديده بالفرونت
                }).ToList();

            var ProviderliveQueueList = liveQueueRepository
                .GetLiveQueueDetailsForShift(shiftId).Where(lq => lq.AppointmentStatus != QueueAppointmentStatus.Completed)//we dont get the completed lq from db 
                .Select(lq => new GetQueueEntriesDTO
                {
                    FullName = lq.Appointment.User != null
                    ? $"{lq.Appointment.User.Client.FirstName ?? ""} {lq.Appointment.User.Client.LastName ?? ""}"
                    : $"{lq.Appointment.TemporaryClient.FirstName ?? ""} {lq.Appointment.TemporaryClient.LastName ?? ""}",

                    ArrivalTime = lq.ArrivalTime,
                    AppointmentDate = lq.Appointment.AppointmentDate,
                    ClientType = lq.Appointment.ClientType,
                    Status = lq.AppointmentStatus,
                    PhoneNumber = lq.Appointment.User != null ? lq.Appointment.User.PhoneNumber ?? ""
                        : lq.Appointment.TemporaryClient.ContactInfo ?? "",
                    CurrentQueuePosition = lq.CurrentQueuePosition

                }).ToList();
            await hubContext.Clients//.All
                .Group($"shift_{shiftId}")
                .SendAsync("QueueUpdated", liveQueueList);

            var providerUserId = shiftRepository.GetShiftById(shiftId)?.ProviderAssignment?.Provider?.ProviderId;

            if (!string.IsNullOrEmpty(providerUserId))
            {
                var providerCconnectionId = QueueHub.GetConnectionId(providerUserId);

                if (providerCconnectionId != null)
                {

                    await hubContext.Clients
                        .Client(providerCconnectionId)
                        .SendAsync("ProviderQueueUpdated", ProviderliveQueueList);
                }
            }
        }

        public async Task<bool> NotifyTurnChange(int liveQueueId, int newQueuePosition)
        {
            var liveQueue = liveQueueRepository.GetById(lq => lq.LiveQueueId == liveQueueId);
            if (liveQueue == null)
            {
                return false;
            }

            var appointment = liveQueue.Appointment;
            if (appointment == null || appointment.User == null)
            {
                return false;
            }
            var user = appointment.User;
            var provider = appointment.Shift.ProviderAssignment.Provider;

            var turnChangeNotification = new Notification
            {
                Title = "Queue Position Changed",
                Message = $"Dear {user.Client.FirstName} {user.Client.LastName}, " +
                         $"Your appointment with Dr. {provider.FirstName} {provider.LastName} on " +
                         $"{appointment.AppointmentDate.ToShortDateString()} has been moved to queue number: {newQueuePosition}. " +
                         "Please contact us if you have any questions.",
                CreatedAt = DateTime.Now,
                IsRead = false,
                UserId = user.Id,
                AppointmentId = appointment.AppointmentId
            };

            user.Notifications.Add(turnChangeNotification);
            commitData.SaveChanges();

            var clientConnectionId = notificationSignalRService.SendMessage(user.Id, new NotificationDTO
            {
                NotificationId = turnChangeNotification.NotificationId,
                Title = turnChangeNotification.Title,
                CreatedAt = turnChangeNotification.CreatedAt,
                Message = turnChangeNotification.Message,
                IsRead = turnChangeNotification.IsRead
            });
            var ClientpaginatedNotifications = notificationServices.GetNotification(user.Id);
            await notificationSignalRService.SendUpdatedNotificationList(user.Id, ClientpaginatedNotifications);

            return true;

        }

        public async Task<PaginationViewModel<ProviderLiveQueueViewModel>> editTurn(int shiftId, int currentQueuePosition)
        {

            var shift = shiftRepository.GetShiftById(shiftId);
            List<LiveQueue> liveQueues = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition > currentQueuePosition).ToList();

            LiveQueue next = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition == currentQueuePosition + 1).FirstOrDefault();

            bool flag = true;

            // case 1: waiting 
            // case 2: not checked -> shift 1 turn - replace with the first waiting turn
            // 1 2 3 4 -> 1 4 3 2

            if (next != null)
            {

                if (next.AppointmentStatus == QueueAppointmentStatus.Waiting)
                {
                    return GetLiveQueuesForProvider(shift.ProviderAssignment.CenterId, shiftId, 1, 16);
                }

                else
                {
                    foreach (LiveQueue lq in liveQueues)
                    {

                        if (lq.AppointmentStatus == QueueAppointmentStatus.Waiting)
                        {
                            flag = false;

                            var position = lq.CurrentQueuePosition ?? 0;

                            reOrder(lq, lq.ShiftId, position, currentQueuePosition + 1);
                            await NotifyTurnChange(lq.LiveQueueId, currentQueuePosition + 1);

                            return GetLiveQueuesForProvider(shift.ProviderAssignment.CenterId, shiftId, 1, 16);

                        }

                    }


                    if (flag)
                    {
                        var currentQueue = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition == currentQueuePosition).FirstOrDefault();
                        if (currentQueue != null)
                        {
                            await NotifyTurnChange(currentQueue.LiveQueueId, currentQueuePosition);
                        }
                        return GetLiveQueuesForProvider(shift.ProviderAssignment.CenterId, shiftId, 1, 16);
                    }

                    return GetLiveQueuesForProvider(shift.ProviderAssignment.CenterId, shiftId, 1, 16);


                }

            }

            else
            {
                var currentQueue = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition == currentQueuePosition).FirstOrDefault();
                if (currentQueue != null)
                {
                    await NotifyTurnChange(currentQueue.LiveQueueId, currentQueuePosition);
                }
                return GetLiveQueuesForProvider(shift.ProviderAssignment.CenterId, shiftId, 1, 16);

            }


        }

        public void reOrder(LiveQueue lq, int shiftId, int positionNumber, int currentQueuePosition)
        {
            LiveQueue late = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition == currentQueuePosition).FirstOrDefault();
            if (late == null)
            {
                return;
            }

            late.CurrentQueuePosition = positionNumber;

            lq.CurrentQueuePosition = currentQueuePosition;

            liveQueueRepository.Edit(late);

            liveQueueRepository.Edit(lq);

            commitData.SaveChanges();

            NotifyTurnChange(late.LiveQueueId, positionNumber).GetAwaiter().GetResult();

        }
    }
}