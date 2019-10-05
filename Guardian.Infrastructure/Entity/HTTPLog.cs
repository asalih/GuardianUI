using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class HTTPLog : EntityBase, IHTTPLog
    {
        public Guid TargetId { get; set; }
        public virtual Target Target { get; set; }
        public string RequestUri { get; set; }
        public int StatusCode { get; set; }
        public long RequestRulesCheckElapsed { get; set; }
        public long ResponseRulesCheckElapsed { get; set; }

        public long HttpElapsed { get; set; }
        public long RequestSize { get; set; }
        public long ResponseSize { get; set; }
    }
}
