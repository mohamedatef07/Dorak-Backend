using System;
using System.Collections.Generic;
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
        public AppointmentRepository(DorakContext _dbContext) : base(_dbContext) { }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByClientId(string clientId)
        {
            return await Table.Where(a => a.UserId == clientId).ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetAppointmentsByProviderId(string providerId)
        {
            return await Table.Where(a => a.ProviderId == providerId).ToListAsync();
        }
        public async Task<IEnumerable<Appointment>> GetUpcomingAppointments()
        {
            return await Table.Where(a=>a.AppointmentDate >= DateTime.Now).ToListAsync();
        }

    }
}
