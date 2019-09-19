using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IFirewallRule : IEntityBase
    {
        Guid AccountId { get; set; }

        Guid TargetId { get; set; }

        string Title { get; set; }

        string Expression { get; set; }

        bool IsActive { get; set; }
    }
}
