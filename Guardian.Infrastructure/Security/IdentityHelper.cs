using Guardian.Infrastructure.Security.Specs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Guardian.Infrastructure.Security
{
    public class IdentityHelper : IIdentityHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal CreateIdentity(Entity.Account account)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, account.FullName, ClaimValueTypes.String),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, account.Email, ClaimValueTypes.String),
                new Claim(ClaimTypes.Role, account.Role, ClaimValueTypes.String)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "User", "Role"));
        }

        public ClaimsPrincipal GetCurrentIdenity() => _httpContextAccessor.HttpContext.User as ClaimsPrincipal;

        public string GetCurrentUsername() => GetCurrentIdenity().Claims.FirstOrDefault(s => s.Type == ClaimTypes.Name).Value;

        public Guid? GetAccountId()
        {
            var value = GetCurrentIdenity()?.Claims?.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;

            return value == null ? null : Guid.Parse(value) as Guid?;
        }
    }
}
