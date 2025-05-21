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
using Repositories;

namespace Services
{
    public class ShiftServices
    {
        ShiftRepository shiftRepository;
        CenterRepository centerRepository;
        AppointmentRepository appointmentRepository;
        LiveQueueRepository liveQueueRepository;
        public CommitData commitData;

        public ShiftServices(ShiftRepository _shiftRepository,CenterRepository _centerRepository, AppointmentRepository _appointmentRepository, LiveQueueRepository _liveQueueRepository, CommitData _commitData)
        {
            shiftRepository = _shiftRepository;
            centerRepository = _centerRepository;
            appointmentRepository = _appointmentRepository;
            liveQueueRepository = _liveQueueRepository;
            commitData = _commitData;
        }

        public IQueryable<ShiftDTO> GetShiftsWithDateAndCenterId(DateOnly _shiftDate, int centerId)
        {
            var shifts = shiftRepository.GetShiftsWithDateAndCenterId(_shiftDate, centerId);
            return shifts.Select(shift => shift.ShiftToShiftVM());
        }

        public IQueryable<AppointmentDTO> GetAppointmentByShiftId(int ShiftId)
        {
            var appointments = appointmentRepository.GetAllAppointmentForShift(ShiftId);
            return appointments.Select(app => app.AppointmentToAppointmentDTO());
        }

        public IQueryable<Appointment> LiveShiftAppointments()
        {   
            var shifts = shiftRepository.LiveQueueShift().ToList();
            foreach (var shift in shifts) {
                var appointments = appointmentRepository.GetAllAppointmentForShift(shift.ShiftId);
                foreach (var appointment in appointments) {

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
    }

}
