using Models.Enums;
using System;


namespace Models.Models
{
    public class Service
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public ServicePriority Priority { get; set; }
        public decimal BasePrice { get; set; }
        


        //Amgad

        public Appointment Appointment { get; set; }
    }
}
