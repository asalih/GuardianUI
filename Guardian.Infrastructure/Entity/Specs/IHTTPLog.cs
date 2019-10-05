using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IHTTPLog : IEntityBase
    {
        Guid TargetId { get; set; }

        string RequestUri { get; set; }

        int StatusCode { get; set; }

        long RequestRulesCheckElapsed { get; set; }

        long ResponseRulesCheckElapsed { get; set; }

        long HttpElapsed { get; set; }

        long RequestSize { get; set; }

        long ResponseSize { get; set; }
    }
}
