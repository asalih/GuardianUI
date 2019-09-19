using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class FirewallRule : EntityBase, IFirewallRule
    {
        public Guid AccountId { get; set; }
        public virtual Account Account { get; set; }
        public Guid TargetId { get; set; }
        public virtual Target Target { get; set; }
        public string Title { get; set; }
        public string Expression { get; set; }
        public bool IsActive { get; set; }
    }
}
