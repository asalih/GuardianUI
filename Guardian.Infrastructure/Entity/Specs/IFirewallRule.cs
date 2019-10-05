using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IFirewallRule : IEntityBase
    {
        RuleFor RuleFor { get; set; }

        Guid AccountId { get; set; }

        Guid TargetId { get; set; }

        RuleAction Action { get; set; }

        string Title { get; set; }

        string Expression { get; set; }

        bool IsActive { get; set; }
    }
}
