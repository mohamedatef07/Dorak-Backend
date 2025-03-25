using Data;
using Dorak.Models;

namespace Repositories
{
    public class CenterRepository : BaseRepository<Center>
    {
        public CenterRepository(DorakContext context):base(context)
        {
            
        }
        public bool AssignProviderToCenter(int providerId, int centerId)
        {
            return true;
        }

    }
}
