using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;

namespace Repositories
{
    public class OperatorRepository : BaseRepository<Operator>
    {
        public OperatorRepository(DorakContext _dbContext) : base(_dbContext)
        {
        }
    }
}
