using Data;
using Dorak.Models;

namespace Repositories
{
    public class ProviderAssignmentRepository : BaseRepository<ProviderAssignment>
    {
        public ProviderAssignmentRepository(DorakContext context) : base(context)
        {

        }
       

        public bool AssignProviderToShift(int providerId, int shiftId)
        {
            if (providerId > 0)
            {
                return true;
            }

            return false; 
        }

    }
}
