using Guardian.Infrastructure.Entity.Specs;
using System;
using System.ComponentModel.DataAnnotations;

namespace Guardian.Infrastructure.Entity
{
    public class Target : EntityBase, ITarget
    {
        [StringLength(250)]
        public string Domain { get; set; }

        [StringLength(250)]
        public string OriginIpAddress { get; set; }

        public string CertKey { get; set; }

        public string CertCrt { get; set; }

        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }

        public bool UseHttps { get; set; }

        public bool WAFEnabled { get; set; }

        public bool IsVerified { get; set; }
    }
}
