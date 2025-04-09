using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public  class CommitData
    {
        private DorakContext context;
        public CommitData(DorakContext _context)
        {
            if (context == null)
            {
                context = _context;
            }
        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
