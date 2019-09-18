using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class WafRule : EntityBase, IWafRule
    {
        public Guid AccountId { get; set; }
        public Guid TargetId { get; set; }
        public string Title { get; set; }
        public string Expression { get; set; }
    }
}
