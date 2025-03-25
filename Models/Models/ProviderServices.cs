using System;


namespace Models.Models
{

    public class ProviderService
    {
        public int ID { get; set; }
        public int ProviderID { get; set; }
        public int ServiceID { get; set; }
        public decimal CustomPrice { get; set; }

        public virtual Provider Provider { get; set; }
        public virtual Service Service { get; set; }
    }


}