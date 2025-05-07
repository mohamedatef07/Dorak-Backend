using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
    }
}
