using Data;
using Dorak.Models;

namespace Repositories
{
    public class ProviderAssignmentRepository : BaseRepository<ProviderAssignment>
    {
        public ProviderAssignmentRepository(DorakContext context) : base(context)
        { }
        public ProviderAssignment GetAssignmentByProviderId(string providerId) { 
        
            var Assignment = GetById(A=>A.ProviderId==providerId);
            return Assignment;
        }

    }
}
