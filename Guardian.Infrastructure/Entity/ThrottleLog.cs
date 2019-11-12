using System;
using Guardian.Infrastructure.Entity.Specs;

namespace Guardian.Infrastructure.Entity
{
    public class ThrottleLog : IThrottleLog
    {
        public Guid Id { get; set; }
        public string IPAddress { get; set; }
        public ThrottleLogType ThrottleType { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
