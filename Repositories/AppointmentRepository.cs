using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class AppointmentRepository : BaseRepository<Appointment>
    {
        public CommitData CommitData;

        public AppointmentRepository(DorakContext _dbContext, CommitData _commitData) : base(_dbContext)
        {
            CommitData = _commitData;
        }

        public IQueryable<Appointment> GetAppointmentsByClientId(string clientId)
        {
            return Table.Where(a => a.UserId == clientId);
        }
        public IQueryable<Appointment> GetAppointmentsByTempClientId(int tempId)
        {
            return Table.Where(a => a.TemporaryClientId == tempId);
        }
        public IEnumerable<Appointment> GetAppointmentsByProviderId(string providerId)
        {
            return Table.Where(a => a.ProviderCenterService.ProviderId == providerId).ToList();
        }
        public IEnumerable<Appointment> GetUpcomingAppointments()
        {
            return Table.Where(a => a.AppointmentDate >= DateOnly.FromDateTime(DateTime.Now)).ToList();
        }

        public Appointment CreateAppoinment(Appointment appointment)
        {
            Table.Add(appointment);
            
            return appointment;
        }

        //public void MakeAppointment(AppointmentViewModel appointmentViewModel);

        public IQueryable<Appointment> GetAllShiftAppointments(int ShiftId)
        {
            return GetAll().Where(a => a.ShiftId == ShiftId);
        }


    }
}
