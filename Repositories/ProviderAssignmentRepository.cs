using Data;
using Dorak.Models;
using System.Linq;

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
        public List<ProviderAssignment> GetCurrentAssignmentsForProvider(string providerId)
        {
            var assignments = GetList(pa => pa.ProviderId == providerId && pa.StartDate <= DateTime.Now && pa.EndDate >= DateTime.Now).ToList();
            return assignments;
        }

    }
}
