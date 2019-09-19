﻿using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Domain.WafRuleLog
{
    public class FirewallRuleLogDto : DtoBase, IRuleLog
    {
        public Guid WafRuleId { get; set; }
        public FirewallRuleLogDto FirewallRuleLog { get; set; }
        public bool IsHitted { get; set; }
        public int ExecutionMillisecond { get; set; }
        public Guid? FirewallRuleId { get; set; }
        public LogType LogType { get; set; }
        public string Description { get; set; }
    }
}
