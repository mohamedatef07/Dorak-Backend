using Models.Enums;
using System;

namespace Dorak.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public ServicePriority Priority { get; set; }
        public decimal BasePrice { get; set; }
<<<<<<< HEAD:Models/Models/Service/Service.cs
        public bool IsDeleted { get; set; }
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual ICollection<CenterService> CenterServices { get; set; }
        public virtual ICollection<ProviderService> ProviderServices { get; set; }
=======

        public virtual ICollection<CenterService> CenterServices { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<ProviderService> ProviderServices { get; set; }

>>>>>>> 511f5ff87e7b2e02e673bbed0b71bd85335f9958:Models/Models/Service.cs
    }
}
