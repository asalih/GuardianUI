using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class WafRuleLog : EntityBase, IWafRuleLog
    {
        public Guid WafRuleId { get; set; }
        public virtual WafRule WafRule { get; set; }
        public bool IsHitted { get; set; }
        public int ExecutionMillisecond { get; set; }
    }
}
