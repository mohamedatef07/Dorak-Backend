using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dorak.DataTransferObject
{
    public class GetProviderMainInfoDTO
    {
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public decimal Rate { get; set; }
        public string Image {  get; set; }
    }
}
