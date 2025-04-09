using Dorak.Models;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ProviderCenterServiceRepository : BaseRepository<ProviderCenterService>
    {
        public ProviderCenterServiceRepository(DorakContext context) : base(context) { }
    }
}
