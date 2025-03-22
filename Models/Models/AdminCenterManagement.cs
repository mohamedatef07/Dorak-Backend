using Microsoft.Identity.Client;
using System;


namespace Models.Models
{
    public class AdminCenterManagement
    {
        public int AdminID { get; set; }
        public int AdminCenterManagementID { get; set; }
        public int CenterID { get; set; }
        public virtual Center? Center { get; set; }
    }
}
