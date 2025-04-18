using Data;
using Dorak.Models;
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
        public async Task<List<LiveQueue>> GetByStatusAsync(string status)
        {
            return await Table
                .Where(q => q.Status.ToLower() == status.ToLower())
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
                .Where(q => q.Status.ToLower() == "done")
                .OrderBy(q => q.CurrentQueuePosition)
                .ToListAsync();
        }

        // count total done clients
        public async Task<int> CountDoneClientsAsync()
        {
            return await Table
                .CountAsync(q => q.Status.ToLower() == "done");
        }

        // get all waiting clients
        public async Task<List<LiveQueue>> GetWaitingClientsAsync()
        {
            return await Table
                .Where(q => q.Status.ToLower() == "waiting")
                .OrderBy(q => q.CurrentQueuePosition)
                .ToListAsync();
        }

        // count total waiting clients
        public async Task<int> CountWaitingClientsAsync()
        {
            return await Table
                .CountAsync(q => q.Status.ToLower() == "waiting");
        }

        // get all not checked clients
        public async Task<List<LiveQueue>> GetNotCheckedClientsAsync()
        {
            return await Table
                .Where(q => q.Status.ToLower() == "not checked")
                .OrderBy(q => q.ArrivalTime)
                .ToListAsync();
        }

        // count total not checked clients
        public async Task<int> CountNotCheckedClientsAsync()
        {
            return await Table
                .CountAsync(q => q.Status.ToLower() == "not checked");
        }

        // get next client in queue
        public async Task<LiveQueue?> GetNextClientAsync()
        {
            return await Table
                .Where(q => q.Status.ToLower() == "waiting")
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

        public void UpdateStatus(int liveQueueId, string newStatus)
        {
            var queueEntry = Table.Find(liveQueueId);
            if (queueEntry != null)
            {
                queueEntry.Status = newStatus;
                commitData.SaveChanges();
            }
        }



    }

}
