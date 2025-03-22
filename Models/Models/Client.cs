using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Models.Models
{
    public class Client
    {
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public string Address { get; set; }
        //public string Street { get; set; } 
        //public string City { get; set; }   
        //public string Governorate { get; set; } 
        //public string Country { get; set; }

        public DateOnly BirthDate { get; set; }
        public string? Image {  get; set; }
        
    }
}
