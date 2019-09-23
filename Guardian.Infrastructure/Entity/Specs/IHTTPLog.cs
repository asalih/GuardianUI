using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IHTTPLog : IEntityBase
    {
        Guid TargetId { get; set; }

        string RequestUri { get; set; }

        int StatusCode { get; set; }

        long RuleCheckElapsed { get; set; }

        long HttpElapsed { get; set; }

        long RequestSize { get; set; }

        long ResponseSize { get; set; }
    }
}
