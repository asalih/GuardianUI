using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IWafRule : IEntityBase
    {
        Guid AccountId { get; set; }

        Guid TargetId { get; set; }

        string Title { get; set; }

        string Expression { get; set; }
    }
}
