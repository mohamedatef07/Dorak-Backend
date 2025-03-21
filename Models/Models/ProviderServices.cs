using System;


namespace Models.Models
{

    public class ProviderService
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Provider")]
        public int ProviderID { get; set; }

        [ForeignKey("Service")]
        public int ServiceID { get; set; }

        public decimal CustomPrice { get; set; }

        public Provider Provider { get; set; }
        public Service Service { get; set; }
    }


}