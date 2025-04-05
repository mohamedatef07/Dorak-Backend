using Dorak.Models;
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ProviderScheduleRepository : BaseRepository<ProviderSchedule>
    {
        public ProviderScheduleRepository(DorakContext context) : base(context)
        {

        }
    }
}
