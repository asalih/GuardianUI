using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IThrottleLog : IEntityBase
    {
        string IPAddress { get; set; }

        ThrottleLogType ThrottleType { get; set; }
    }
}
