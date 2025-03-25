using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Repositories;

namespace Dorak.Models.Models.Wallet
{
    public class ShiftRepository : BaseRepository<Shift>
    {
        //public DorakContext context { get; set; }
        public ShiftRepository(DorakContext _context) : base(_context)
        { 
            //context = _context;
        }

}
}
