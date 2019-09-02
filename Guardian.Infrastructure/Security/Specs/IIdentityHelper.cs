using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Guardian.Infrastructure.Security.Specs
{
    public interface IIdentityHelper
    {
        ClaimsPrincipal CreateIdentity(Infrastructure.Entity.Account account);

        ClaimsPrincipal GetCurrentIdenity();

        string GetCurrentUsername();

        Guid? GetAccountId();
    }
}
