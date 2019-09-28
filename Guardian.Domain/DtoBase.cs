using System;

namespace Guardian.Domain
{
    public class DtoBase
    {
        public DtoBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
