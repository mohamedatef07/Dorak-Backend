using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class AddUserToNotificationHubDTO
    {
       
            public string UserId { get; set; }
            public string ConnectionId { get; set; }
            public string Role { get; set; }
            public string Name { get; set; }
        
    }
}
