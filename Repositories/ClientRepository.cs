using Data;
using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ClientRepository : BaseRepository<Client>
    {
        public ClientRepository(DorakContext context) : base(context) { }
    }
}
