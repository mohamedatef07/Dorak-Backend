using Data;
using Dorak.Models;

namespace Repositories
{
    public class NotificationRepository : BaseRepository<Notification>
    {
        public NotificationRepository(DorakContext _dbContext) : base(_dbContext)
        {
        }
    }
}
