using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Dorak.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(DorakContext dbContext) : base(dbContext)
        {
        }
        
    }
}
