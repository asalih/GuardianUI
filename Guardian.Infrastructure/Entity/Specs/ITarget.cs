using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface ITarget : IEntityBase
    {
        string Domain { get; set; }

        string OriginIpAddress { get; set; }

        string CertKey { get; set; }

        string CertCrt { get; set; }

        bool UseHttps { get; set; }

        bool WAFEnabled { get; set; }

        Protocol Proto { get; set; }

        TargetState State { get; set; }
    }
}
