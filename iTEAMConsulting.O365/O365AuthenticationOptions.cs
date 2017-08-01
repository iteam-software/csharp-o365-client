using System;
using System.Collections.Generic;
using System.Text;

namespace iTEAMConsulting.O365
{
    public class O365AuthenticationOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantName { get; set; }
        public string TenantId { get; set; }
        public byte[] CertBytes { get; set; }
        public string CertPrivateKey { get; set; }
        public string FromAddress { get; set; }
    }
}
