using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Entity.Specs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Guardian.Domain.Target
{
    public class TargetDto : DtoBase, ITarget
    {
        public TargetDto()
        {
            UseHttps = true;
            CreatedAt = DateTimeOffset.Now;
        }

        public string Domain { get; set; }

        [Display(Name = "Origin IP Address")]
        public string OriginIpAddress { get; set; }

        [Display(Name = "Certificate Key")]
        public string CertKey { get; set; }

        [Display(Name = "Certificate")]
        public string CertCrt { get; set; }

        [Display(Name = "Force to Use Https")]
        public bool UseHttps { get; set; }

        [Display(Name = "WAF Enabled")]
        public bool WAFEnabled { get; set; }

        public int ActiveFirewallRulesCount { get; set; }

        public int PassiveFirewallRulesCount { get; set; }

        [Display(Name = "Protocol between Guardian and the target")]
        public Protocol Proto { get; set; }

        public TargetState State { get; set; }

        [Display(Name = "Enable Auto Certification")]
        public bool AutoCert { get; set; }

        [Display(Name = "Create Self Signed Certificate")]
        public bool CreateSelfSignedCertificate { get; set; }
    }
}
