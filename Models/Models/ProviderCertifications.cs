using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models
{
    public class ProviderCertifications
    {
        public int ID { get; set; }
        public string Certification {  get; set; }
        public int ProviderID { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
