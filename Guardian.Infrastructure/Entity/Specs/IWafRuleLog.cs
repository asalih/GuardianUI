using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IWafRuleLog : IEntityBase
    {
        Guid WafRuleId { get; set; }

        bool IsHitted { get; set; }

        int ExecutionMillisecond { get; set; }
    }
}
