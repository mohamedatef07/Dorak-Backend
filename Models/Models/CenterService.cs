using System;


namespace Models.Models
{
    public class CenterService
    {
        public int CenterServiceID { get; set; }
        public int CenterID { get; set; }
        public virtual Center Center { get; set; }
        public int ServiceID { get; set; }
        public virtual Service Service { get; set; }
    }
}
