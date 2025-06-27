using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Models.Enums;
using Microsoft.EntityFrameworkCore;
using Dorak.Models;

namespace Repositories
{
    public class ShiftRepository : BaseRepository<Shift>
    {
        public ShiftRepository(DorakContext _context) : base(_context){ }

        public  Shift GetShiftById(int shiftId)
        {
            return Table.Where(shift => shift.ShiftId == shiftId).FirstOrDefault();
        }
        public async Task<IEnumerable<Shift>> GetShiftsByProviderId(string providerId)
        {
            return await Table.Where(s => s.ProviderAssignment.ProviderId == providerId).ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetShiftsByCenterId(int centerId)
        {
            return await Table.Where(s=>s.ProviderAssignment.CenterId == centerId).ToListAsync();
        }
        public Shift GetShiftByAssignmentId(int ProviderAssignmentId)
        {
            return Table.Where(s => s.ProviderAssignmentId == ProviderAssignmentId).FirstOrDefault();
        }

        public IQueryable<Shift> GetShiftsWithDateAndCenterId(DateOnly _date, int _centerId)
        {
            return GetAll().Where(s => s.ShiftDate == _date).Where(s => s.ProviderAssignment.CenterId == _centerId);
        }

        public IQueryable<Shift> GetShiftsWithDateAndProvider(DateOnly _date, string providerId)
        {
            return GetAll().Where(s => s.ShiftDate == _date && s.ProviderAssignment.ProviderId == providerId);
        }

        public List<Shift> GetAllShiftsByAssignmentId(int ProviderAssignmentId)
        {
            return Table.Where(s => s.ProviderAssignmentId == ProviderAssignmentId).ToList();
        }

        public IQueryable<Shift> LiveQueueShift()
        {
            var now = DateTime.Now;
            var today = DateOnly.FromDateTime(now.Date);
            var currentTime = TimeOnly.FromDateTime(now);

            return Table.Where(lq => lq.ShiftDate <= today &&
                                    lq.StartTime <= currentTime &&
                                    lq.ShiftType != ShiftType.Completed);
        }
    }
}