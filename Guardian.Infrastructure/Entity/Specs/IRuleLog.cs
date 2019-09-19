using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IRuleLog : IEntityBase
    {
        Guid? FirewallRuleId { get; set; }

        bool IsHitted { get; set; }

        int ExecutionMillisecond { get; set; }

        LogType LogType { get; set; }

        string Description { get; set; }
    }
}
