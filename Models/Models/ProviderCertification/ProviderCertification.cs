using System;

namespace Dorak.Models
{
    public class ProviderCertification
    {
        public int ProviderCertificationsId { get; set; }
        public string Certification {  get; set; }
        public string ProviderId { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Provider Provider { get; set; }
    }
}
