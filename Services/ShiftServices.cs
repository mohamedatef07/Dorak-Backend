using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
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
        AppointmentRepository _appointmentRepository;
        public CommitData commitData;

        public ShiftServices(ShiftRepository _shiftRepository,CenterRepository _centerRepository, AppointmentRepository appointmentRepository, CommitData _commitData)
        {
            shiftRepository = _shiftRepository;
            centerRepository = _centerRepository;
            _appointmentRepository = appointmentRepository;
            commitData = _commitData;
        }

        public IQueryable<ShiftDTO> GetShiftsWithDateAndCenterId(DateTime _shiftDate, int centerId)
        {
            var shifts = shiftRepository.GetShiftsWithDateAndCenterId(_shiftDate,centerId);
            return shifts.Select(shift => shift.ShiftToShiftVM());
        }

        public IQueryable<AppointmentDTO> GetAppointmentByShiftId(int ShiftId)
        {
            var appointments = _appointmentRepository.GetAllAppointmentForShift(ShiftId);
            return appointments.Select(app => app.AppointmentToAppointmentDTO());
        }

        //public IQueryable<Appointment> LiveShiftAppointments()
        //{
        //    var
        //    var LiveQueue = shiftRepository.get
        //}
    }

}
