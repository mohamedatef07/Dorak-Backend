using Data;
using Dorak.Models;

namespace Repositories
{
    public class ProviderAssignmentRepository : BaseRepository<ProviderAssignment>
    {
        public ProviderAssignmentRepository(DorakContext context) : base(context)
        {

        }
    }
}
