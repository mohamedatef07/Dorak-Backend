using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject.AccountDTO
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public bool LogoutAllDevices { get; set; }

    }
}
