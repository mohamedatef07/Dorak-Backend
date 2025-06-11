using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.DataTransferObject;
using Dorak.Models;
using Dorak.Models.Models.Wallet;
using Dorak.ViewModels;
using Models.Enums;
using Repositories;

namespace Services
{
    public class ShiftServices
    {
        private readonly ShiftRepository shiftRepository;
        private readonly AppointmentRepository appointmentRepository;
        private readonly LiveQueueRepository liveQueueRepository;


        public ShiftServices(ShiftRepository _shiftRepository, AppointmentRepository _appointmentRepository, LiveQueueRepository _liveQueueRepository)
        {
            shiftRepository = _shiftRepository;
            appointmentRepository = _appointmentRepository;
            liveQueueRepository = _liveQueueRepository;
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
                     pa.Shifts.Select(shift => new GetAllCenterShiftsDTO
                     {
                         ProviderName = $"{pa.Provider.FirstName} {pa.Provider.LastName}",
                         ShiftId = shift.ShiftId,
                         ShiftDate = shift.ShiftDate,
                         StartTime = shift.StartTime,
                         EndTime = shift.EndTime,
                     })
                ).ToList();
            return shifts;
        }
        public bool ShiftCancelation(Shift shift)
        {
            if (shift == null)
            {
                return false;
            }
            shift.ShiftType = ShiftType.Cancelled;
            foreach(var appointment in shift.Appointments)
            {
                appointment.AppointmentStatus = AppointmentStatus.Cancelled;
            }
            shiftRepository.Edit(shift);
            return true;
        }
    }


}
