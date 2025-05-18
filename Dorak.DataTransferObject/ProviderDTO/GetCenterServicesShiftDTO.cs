using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject.ProviderDTO
{
       public class GetProviderCenterServicesDTO
    {
        public int CenterId {  get; set; }
        public string CenterName { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public List<GetProviderSrvicesDTO> Services { get; set; }
    }
}
