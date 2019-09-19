using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class RuleLog : EntityBase, IRuleLog
    {
        public virtual FirewallRule WafRule { get; set; }
        public bool IsHitted { get; set; }
        public int ExecutionMillisecond { get; set; }
        public LogType LogType { get; set; }
        public string Description { get; set; }
        public Guid? FirewallRuleId { get; set; }
    }
}
