using Guardian.Infrastructure.Entity.Specs;
using System;

namespace Guardian.Infrastructure.Entity
{
    public class EntityBase : IEntityBase
    {
        public EntityBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
