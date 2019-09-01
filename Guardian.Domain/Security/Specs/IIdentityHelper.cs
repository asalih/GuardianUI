using System;
using System.Security.Claims;

namespace Guardian.Domain.Security.Specs
{
    public interface IIdentityHelper
    {
        ClaimsPrincipal CreateIdentity(Infrastructure.Entity.Account account);

        ClaimsPrincipal GetCurrentIdenity();

        string GetCurrentUsername();

        Guid GetAccountId();
    }
}
