using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LiveQueueServices
    {
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        private readonly CommitData commitData;
        private readonly IHubContext<QueueHub> hubContext;

        public LiveQueueServices(LiveQueueRepository _liveQueueRepository,
            AppointmentRepository appointmentRepository,
            ProviderServices _providerServices,
            ClientRepository _clientRepository,
            ProviderAssignmentRepository _providerAssignmentRepository,
            OperatorRepository _operatorRepository,
            IHubContext<QueueHub> hubContext, 
            CommitData _commitData)
        {
            liveQueueRepository = _liveQueueRepository;
            this.appointmentRepository = appointmentRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
            this.hubContext = hubContext;
            commitData = _commitData;
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
        public PaginationViewModel<ProviderLiveQueueViewModel> GetLiveQueuesForProvider(string providerId, int centerId, int shiftId, int pageNumber = 1, int pageSize = 16)
        {
            var providerAssignments = providerAssignmentRepository.GetAll()
                .Where(pa => pa.ProviderId == providerId && pa.CenterId == centerId && !pa.IsDeleted)
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

            // Filter shifts by the specific shiftId
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
                        AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>()
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
                        AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>()
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

            var LqAppointment = liveQueueRepository.Get(app => app.AppointmentId == appointmentId).FirstOrDefault();
            if (LqAppointment == null) {
                return null;
            }
            var result = liveQueues.Select(lq => new ClientLiveQueueDTO
            {

                ArrivalTime = lq.ArrivalTime,
                AppointmentDate = lq.Appointment.AppointmentDate,
                Type = lq.Appointment.ClientType,
                Status = lq.AppointmentStatus,

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

                    // 🔥 This flag helps you identify the logged-in client's appointment
                    IsCurrentClient = appointment.UserId == userId
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task NotifyShiftQueueUpdate(int shiftId)
        {
            var liveQueueList = liveQueueRepository
                .GetLiveQueueDetailsForShift(shiftId)
                .Select(lq => new ClientLiveQueueDTO
                {

                    ArrivalTime = lq.ArrivalTime,
                    AppointmentDate = lq.Appointment.AppointmentDate,
                    Type = lq.Appointment.ClientType,
                    Status = lq.AppointmentStatus,

                    CurrentQueuePosition = lq.CurrentQueuePosition,
                    IsCurrentClient = false // سيتم تحديده بالفرونت
                }).ToList();

            await hubContext.Clients.All
                //.Group($"shift_{shiftId}")
                .SendAsync("QueueUpdated", liveQueueList);
        }

        //public async Task editTurn()
        //{

        //    List<LiveQueue> liveQueues = liveQueueRepository.GetAll().ToList();

        //    foreach(LiveQueue lq in liveQueues)
        //    {
        //        LiveQueue previous = liveQueueRepository.GetAll().Where(l => l.ShiftId == lq.ShiftId && l.CurrentQueuePosition == lq.CurrentQueuePosition - 1).FirstOrDefault();

        //        if (lq.EstimatedTime <= TimeOnly.FromDateTime(DateTime.Now) && lq.AppointmentStatus == QueueAppointmentStatus.NotChecked && previous.AppointmentStatus == QueueAppointmentStatus.Completed)
        //        {

        //            var position = lq.CurrentQueuePosition ?? 0;


        //            reOrder(lq, lq.ShiftId, position);

                  



        //        }
        //    }

        //}

        public void reOrder (LiveQueue liveQueue, int shiftId, int positionNumber)
        {
            List<LiveQueue> liveQueues = liveQueueRepository.GetAll().Where(l => l.ShiftId == shiftId && l.CurrentQueuePosition > positionNumber).ToList();

            foreach(LiveQueue lq in liveQueues)
            {

                lq.CurrentQueuePosition -= lq.CurrentQueuePosition;

                liveQueueRepository.Edit(lq);

                commitData.SaveChanges();

            }


        }
    }
}