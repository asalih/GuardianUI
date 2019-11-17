using System;

namespace Guardian.Infrastructure.Entity.Specs
{
    public interface IAccount : IEntityBase
    {
        string Email { get; set; }

        string Password { get; set; }

        string Salt { get; set; }

        string FullName { get; set; }

        string Token { get; set; }
    }
}
