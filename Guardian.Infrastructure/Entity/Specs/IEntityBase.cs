using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IEntityBase
    {
        Guid Id { get; set; }

        DateTimeOffset CreatedAt { get; set; }
    }
}
