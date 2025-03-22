using System;


namespace Models.Models
{

    public class ProviderService
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Provider")]

        [ForeignKey("Service")]

        public decimal CustomPrice { get; set; }

        public int ProviderID { get; set; }
        public Provider Provider { get; set; }
        public int ServiceID { get; set; }
        public Service Service { get; set; }
    }


}