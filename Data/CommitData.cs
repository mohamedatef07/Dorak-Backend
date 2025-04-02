using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CommitData
    {
        private static DorakContext context;
        public static void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}
