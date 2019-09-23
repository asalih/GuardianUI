using Guardian.Domain.Target;
using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Domain.HTTPLog
{
    public class HTTPLogDto : DtoBase, IHTTPLog
    {
        public Guid TargetId { get; set; }
        public virtual TargetDto Target { get; set; }
        public string RequestUri { get; set; }
        public int StatusCode { get; set; }
        public long RuleCheckElapsed { get; set; }
        public long HttpElapsed { get; set; }
        public long RequestSize { get; set; }
        public long ResponseSize { get; set; }
    }
}
