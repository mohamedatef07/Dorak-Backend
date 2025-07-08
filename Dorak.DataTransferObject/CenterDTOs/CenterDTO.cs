using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Enums;

namespace Dorak.DataTransferObject
{
    public class CenterDTO_
    {
        public string CenterName { get; set; }
        public string ContactNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string WebsiteURL { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string MapURL { get; set; }
        public bool IsDeleted { get; set; }
        public CenterStatus CenterStatus { get; set; }
    }
}
