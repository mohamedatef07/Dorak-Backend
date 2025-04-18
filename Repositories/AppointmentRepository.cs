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

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClientId(string clientId)
        {
            return await Table.Where(a => a.UserId == clientId).ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByProviderId(string providerId)
        {
            return await Table.Where(a => a.ProviderCenterService.ProviderId == providerId).ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetUpcomingAppointments()
        {
            return await Table.Where(a => a.AppointmentDate >= DateTime.Now).ToListAsync();
        }

        public Appointment CreateAppoinment(Appointment appointment)
        {
            Table.Add(appointment);
            CommitData.SaveChanges();
            return appointment;
        }

        //public void MakeAppointment(AppointmentViewModel appointmentViewModel);

        public IQueryable<Appointment> GetAllAppointmentForShift(int ShiftId)
        {
            return GetAll().Where(a => a.ShiftId == ShiftId);
        }
    }
}
