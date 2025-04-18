using Dorak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject.ShiftDTO
{
    public class GetCenterServicesShiftDTO
    {
        public Center Center {  get; set; }
        public List<Service> Services { get; set; }
    }
}
