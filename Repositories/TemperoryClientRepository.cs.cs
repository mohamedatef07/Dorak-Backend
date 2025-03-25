using Data;
using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TemperoryClientRepository : BaseRepository<TemporaryClient>
    {
        public TemperoryClientRepository(DorakContext context) : base(context) { }

    }
}
