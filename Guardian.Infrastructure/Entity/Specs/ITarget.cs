using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface ITarget : IEntityBase
    {
        string Domain { get; set; }

        string OriginIpAddress { get; set; }

        string CertKey { get; set; }

        string CertCrt { get; set; }
    }
}
