using Guardian.Infrastructure.Entity.Specs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Guardian.Domain.Target
{
    public class TargetDto : ITarget
    {
        public TargetDto()
        {
            CreatedAt = DateTimeOffset.Now;
        }

        public Guid Id { get; set; }

        public string Domain { get; set; }

        [Display(Name = "Origin IP Address")]
        public string OriginIpAddress { get; set; }

        [Display(Name="Certificate Key")]
        public string CertKey { get; set; }

        [Display(Name = "Certificate")]
        public string CertCrt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
