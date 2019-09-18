using System;

namespace Guardian.Domain
{
    public class DtoBase
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
