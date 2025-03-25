using Data;
using Dorak.Models;

namespace Repositories
{
    public class ProviderRepository : BaseRepository<Provider>
    {
        public ProviderRepository(DorakContext context) : base(context)
        {
            
        }
    }
}
