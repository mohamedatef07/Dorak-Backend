using System;
using Data;
using Dorak.Models;

namespace Repositories
{
    public class PaymentRepository : BaseRepository<Payment>
    {
        public PaymentRepository(DorakContext dbContext) : base(dbContext)
        {
        }
        
    }
}
