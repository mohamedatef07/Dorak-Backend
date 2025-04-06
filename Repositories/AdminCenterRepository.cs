using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;

namespace Repositories
{
    public class AdminCenterRepository : BaseRepository<AdminCenterManagement>
    {
        public AdminCenterRepository(DorakContext _dbContext) : base(_dbContext){}
    }
}
