using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ServicesRepository : BaseRepository<Service>
    {
        public ServicesRepository(DorakContext dbContext) : base(dbContext)
        {
        }



    }
}

