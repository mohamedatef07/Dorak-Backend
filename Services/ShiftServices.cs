using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.ViewModels;
using Hubs;
using Microsoft.AspNetCore.SignalR;
using Models.Enums;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ShiftServices
    {
        private readonly ShiftRepository shiftRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly LiveQueueRepository liveQueueRepository;
        private readonly CenterServices centerServices;
        private readonly CommitData commitData;
        private readonly IHubContext<ShiftListHub> shiftListHubContext;


        public ShiftServices(ShiftRepository _shiftRepository, AppointmentRepository _appointmentRepository, LiveQueueRepository _liveQueueRepository, CommitData _commitData, IHubContext<ShiftListHub> _shiftListHubContext, CenterServices _centerServices)
        {
            shiftRepository = _shiftRepository;
            appointmentRepository = _appointmentRepository;
            liveQueueRepository = _liveQueueRepository;
            commitData = _commitData;
            shiftListHubContext = _shiftListHubContext;
            centerServices = _centerServices;
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

        public IQueryable<AppointmentDTO> GetAppointmentByShiftId(int ShiftId)
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
                        //Status = 

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
            if (shift.Appointments != null && shift.Appointments.Any())
            {
                foreach (var appointment in shift.Appointments)
                {
                    appointment.AppointmentStatus = AppointmentStatus.Cancelled;
                    appointment.User.Notifications.Add(new Notification { Title = "Appointment Cancellation", Message = $"Your appointment on {appointment.AppointmentDate.ToShortDateString()} with Dr. {appointment.Shift.ProviderAssignment.Provider.FirstName} {appointment.Shift.ProviderAssignment.Provider.LastName} has been canceled. We apologize for any inconvenience. Please contact us or reschedule your appointment at your earliest convenience." });
                }
                shiftRepository.Edit(shift);
                commitData.SaveChanges();
                var center = centerServices.GetCenterById(centerId);
                var updatedShiftList = GetAllCenterShifts(center);
                await shiftListHubContext.Clients.All.SendAsync("updateShiftsList", updatedShiftList);
                return true;
            }
            return false;
        }
    }


}
