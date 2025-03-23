using System;

namespace Models.Models
{
    public class ProviderCertification
    {
        public int ProviderCertificationsId { get; set; }
        public string Certification {  get; set; }
        public string ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
    }
}
