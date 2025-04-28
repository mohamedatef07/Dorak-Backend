using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;

namespace Repositories
{
    public class ReviewRepository : BaseRepository<Review>
    {
        public ReviewRepository(DorakContext dbContext) : base(dbContext) { }
    }
}
