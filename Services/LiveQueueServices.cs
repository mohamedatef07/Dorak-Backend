using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Models.Enums;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LiveQueueServices
    {
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly ProviderAssignmentRepository providerAssignmentRepository;
        public LiveQueueServices(LiveQueueRepository _liveQueueRepository, ProviderServices _providerServices, ClientRepository _clientRepository, ProviderAssignmentRepository _providerAssignmentRepository, OperatorRepository _operatorRepository)
        {
            liveQueueRepository = _liveQueueRepository;
            providerAssignmentRepository = _providerAssignmentRepository;
        }

        //Get Queue Entries for a provider
        //public List<GetQueueEntriesDTO> GetQueueEntries(Provider provider)
        //{
        //    List<ProviderAssignment> providerAssignments = providerAssignmentRepository.GetCurrentAssignmentsForProvider(provider.ProviderId);
        //    List<Shift> shifts = providerAssignments.SelectMany(pa => pa.Shifts.Where(sh => sh.ProviderAssignmentId == pa.AssignmentId && sh.ShiftType == ShiftType.OnGoing)).ToList();
        //    List<LiveQueue> liveQueues = shifts.SelectMany(sh => sh.LiveQueues.Where(lq => lq.ShiftId == sh.ShiftId)).ToList();
        //    List<GetQueueEntriesDTO> result = new List<GetQueueEntriesDTO>();
        //    foreach (var liveQueue in liveQueues)
        //    {
        //        var appointment = liveQueue.Appointment;
        //        if (appointment.User == null )
        //        {
        //            result.Add(new GetQueueEntriesDTO
        //            {
        //                FullName = $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
        //                ArrivalTime = liveQueue.ArrivalTime,
        //                AppointmentDate = appointment.AppointmentDate,
        //                Type = appointment.ClientType,
        //                Status = liveQueue.AppointmentStatus,
        //                PhoneNumber = appointment.TemporaryClient.ContactInfo,
        //                CurrentQueuePosition = liveQueue.CurrentQueuePosition

        //            });
        //        }
        //        else
        //        {
        //            result.Add(new GetQueueEntriesDTO
        //            {
        //                FullName = $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}",
        //                ArrivalTime = liveQueue.ArrivalTime,
        //                AppointmentDate = appointment.AppointmentDate,
        //                Type = appointment.ClientType,
        //                Status = liveQueue.AppointmentStatus,
        //                PhoneNumber = appointment.User.PhoneNumber,
        //                CurrentQueuePosition = liveQueue.CurrentQueuePosition
        //            });
        //        }
        //    }
        //    return result;
        //}

        //public List<ProviderLiveQueueViewModel> GetLiveQueuesForProvider(string providerId, int centerId)
        //{
            
        //    var providerAssignments = providerAssignmentRepository.GetAll()
        //        .Where(pa => pa.ProviderId == providerId && pa.CenterId == centerId && !pa.IsDeleted)
        //        .ToList();

        //    if (!providerAssignments.Any())
        //    {
                
        //        return new List<ProviderLiveQueueViewModel>();
        //    }

            
        //    List<Shift> shifts = providerAssignments.SelectMany(pa => pa.Shifts
        //        .Where(sh => sh.ProviderAssignmentId == pa.AssignmentId && sh.ShiftType == ShiftType.OnGoing && !sh.IsDeleted))
        //        .ToList();
            

        //    if (!shifts.Any())
        //    {
                
        //        return new List<ProviderLiveQueueViewModel>();
        //    }

            
        //    List<LiveQueue> liveQueues = shifts.SelectMany(sh => sh.LiveQueues
        //        .Where(lq => lq.ShiftId == sh.ShiftId))
        //        .ToList();
            

            
        //    List<ProviderLiveQueueViewModel> result = new List<ProviderLiveQueueViewModel>();
        //    foreach (var liveQueue in liveQueues)
        //    {
        //        var appointment = liveQueue.Appointment;
        //        if (appointment.User == null)
        //        {
        //            result.Add(new ProviderLiveQueueViewModel
        //            {
        //                LiveQueueId = liveQueue.LiveQueueId,
        //                ClientFullName = $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
        //                ArrivalTime = liveQueue.ArrivalTime,
        //                EstimatedTime = liveQueue.EstimatedTime,
        //                ClientType = appointment.ClientType,
        //                Status = liveQueue.AppointmentStatus,
        //                PhoneNumber = appointment.TemporaryClient.ContactInfo,
        //                CurrentQueuePosition = liveQueue.CurrentQueuePosition,
        //                AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>()
        //            });
        //        }
        //        else
        //        {
        //            result.Add(new ProviderLiveQueueViewModel
        //            {
        //                LiveQueueId = liveQueue.LiveQueueId,
        //                ClientFullName = $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}",
        //                ArrivalTime = liveQueue.ArrivalTime,
        //                EstimatedTime = liveQueue.EstimatedTime,
        //                ClientType = appointment.ClientType,
        //                Status = liveQueue.AppointmentStatus,
        //                PhoneNumber = appointment.User.PhoneNumber,
        //                CurrentQueuePosition = liveQueue.CurrentQueuePosition,
        //                AvailableStatuses = Enum.GetValues(typeof(QueueAppointmentStatus)).Cast<QueueAppointmentStatus>()
        //            });
        //        }
        //    }

        //    return result;
        //    // return result.OrderBy(lq => lq.CurrentQueuePosition).ToList();
        //}

        public PaginationViewModel<ProviderLiveQueueViewModel> GetLiveQueuesForProvider(string providerId, int centerId, int pageNumber = 1, int pageSize = 16)
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

            
            List<Shift> shifts = providerAssignments.SelectMany(pa => pa.Shifts
                .Where(sh => sh.ProviderAssignmentId == pa.AssignmentId && sh.ShiftType == ShiftType.OnGoing && !sh.IsDeleted))
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


        //public async Task<GetQueueEntriesDTO?> GetQueueEntryByAppointmentIdAsync(int appointmentId)
        //{
        //    var liveQueue = await liveQueueRepository.GetByAppointmentIdAsync(appointmentId);

        //    if (liveQueue == null)
        //        return null;

        //    var appointment = liveQueue.Appointment;

        //    return new GetQueueEntriesDTO
        //    {
        //        FullName = appointment.User != null
        //            ? $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}"
        //            : $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
        //        ArrivalTime = liveQueue.ArrivalTime,
        //        AppointmentDate = appointment.AppointmentDate,
        //        Type = appointment.ClientType,
        //        Status = liveQueue.AppointmentStatus,
        //        PhoneNumber = appointment.User != null
        //            ? appointment.User.PhoneNumber
        //            : appointment.TemporaryClient.ContactInfo,
        //        CurrentQueuePosition = liveQueue.CurrentQueuePosition
        //    };
        //}

        public async Task<List<ClientLiveQueueDTO>> GetLiveQueueForShiftAsync(int shiftId, string? userId)
        {
            var liveQueues = await liveQueueRepository.GetLiveQueueDetailsForShiftAsync(shiftId);
            var result = new List<ClientLiveQueueDTO>();

            foreach (var liveQueue in liveQueues)
            {
                var appointment = liveQueue.Appointment;

                var dto = new ClientLiveQueueDTO
                {
                    FullName = appointment.User != null
                        ? $"{appointment.User.Client.FirstName} {appointment.User.Client.LastName}"
                        : $"{appointment.TemporaryClient.FirstName} {appointment.TemporaryClient.LastName}",
                    ArrivalTime = liveQueue.ArrivalTime,
                    AppointmentDate = appointment.AppointmentDate,
                    Type = appointment.ClientType,
                    Status = liveQueue.AppointmentStatus,
                    PhoneNumber = appointment.User != null
                        ? appointment.User.PhoneNumber
                        : appointment.TemporaryClient.ContactInfo,
                    CurrentQueuePosition = liveQueue.CurrentQueuePosition,

                    // 🔥 This flag helps you identify the logged-in client's appointment
                    IsCurrentClient = appointment.UserId == userId
                };

                result.Add(dto);
            }

            return result;
        }

    }
}
