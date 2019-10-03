using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IRuleLog : IEntityBase
    {
        Guid TargetId { get; set; }

        Guid? FirewallRuleId { get; set; }

        bool IsHitted { get; set; }

        int ExecutionMillisecond { get; set; }

        LogType LogType { get; set; }

        string Description { get; set; }

        string RequestUri { get; set; }

        RuleFor RuleFor { get; set; }
    }
}
