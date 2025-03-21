using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class TemporaryClient
    {
        public int TempClientID {  get; set; }
        public List<string> ContactInfo { get; set; }
        public string TempCode { get; set; }
    }
}
