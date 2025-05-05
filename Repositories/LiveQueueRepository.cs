using Data;
using Dorak.Models;
using Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories
{
    public class LiveQueueRepository : BaseRepository<LiveQueue>
    {
        public CommitData commitData;
        public LiveQueueRepository(DorakContext dbContext, CommitData _commitData) : base(dbContext)
        {
            commitData = _commitData;
        }

        // Get queue by status
        public async Task<List<LiveQueue>> GetByStatusAsync(QueueAppointmentStatus status)
        {
            return await Table
                .Where(q => q.AppointmentStatus == status)
                .ToListAsync();
        }

        // Get queue capacity
        public async Task<int> GetQueueCapacityAsync(string operatorId)
        {
            return await Table
                .Where(q => q.OperatorId == operatorId)
                .CountAsync();
        }

        // Get all done clients
        public async Task<List<LiveQueue>> GetDoneClientsAsync()
        {
            return await Table
                .Where(q => q.AppointmentStatus == QueueAppointmentStatus.Completed)
                .OrderBy(q => q.CurrentQueuePosition)
                .ToListAsync();
        }

        // count total done clients
        public async Task<int> CountDoneClientsAsync()
        {
            return await Table
                .CountAsync(q => q.AppointmentStatus == QueueAppointmentStatus.Completed);
        }

        // get all waiting clients
        public async Task<List<LiveQueue>> GetWaitingClientsAsync()
        {
            return await Table
                .Where(q => q.AppointmentStatus == QueueAppointmentStatus.Waiting)
                .OrderBy(q => q.CurrentQueuePosition)
                .ToListAsync();
        }

        // count total waiting clients
        public async Task<int> CountWaitingClientsAsync()
        {
            return await Table
                .CountAsync(q => q.AppointmentStatus == QueueAppointmentStatus.Waiting);
        }

        // get all not checked clients
        public async Task<List<LiveQueue>> GetNotCheckedClientsAsync()
        {
            return await Table
                .Where(q => q.AppointmentStatus == QueueAppointmentStatus.NotChecked)
                .OrderBy(q => q.ArrivalTime)
                .ToListAsync();
        }

        // count total not checked clients
        public async Task<int> CountNotCheckedClientsAsync()
        {
            return await Table
                .CountAsync(q => q.AppointmentStatus == QueueAppointmentStatus.NotChecked);
        }

        // get next client in queue
        public async Task<LiveQueue?> GetNextClientAsync()
        {
            return await Table
                .Where(q => q.AppointmentStatus == QueueAppointmentStatus.Waiting)
                .OrderBy(q => q.CurrentQueuePosition)
                .FirstOrDefaultAsync();
        }

        // get queue by opeartor
        public async Task<List<LiveQueue>> GetByOperatorIdAsync(string operatorId)
        {
            return await Table
                .Where(q => q.OperatorId == operatorId)
                .OrderBy(q => q.CurrentQueuePosition)
                .ToListAsync();
        }

        // update status

        public void UpdateStatus(int liveQueueId, QueueAppointmentStatus newStatus)
        {
            var queueEntry = Table.Find(liveQueueId);
            if (queueEntry != null)
            {
                queueEntry.AppointmentStatus = newStatus;
                commitData.SaveChanges();
            }
        }

        public int GetCurrentPostion( Appointment appointment)
        {

            var priority = appointment.ProviderCenterService.Priority;
            var count = appointment.Shift.Appointments.Count();
            return count;
        }

        public IQueryable<LiveQueue> GetAllLiveQueueForShift(int ShiftId)
        {
            return GetAll().Where(a => a.ShiftId == ShiftId);
        }

    }

}
