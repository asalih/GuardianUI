using Guardian.Domain.Security.Specs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Linq;

namespace Guardian.Domain.Security
{
    public class IdentityHelper : IIdentityHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal CreateIdentity(Infrastructure.Entity.Account account)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, account.FullName, ClaimValueTypes.String),
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(), ClaimValueTypes.String),
                new Claim(ClaimTypes.Email, account.Email, ClaimValueTypes.String),
                new Claim(ClaimTypes.Role, Constants.RolesUser, ClaimValueTypes.String)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies", "User", "Role"));
        }

        public ClaimsPrincipal GetCurrentIdenity() => _httpContextAccessor.HttpContext.User as ClaimsPrincipal;

        public string GetCurrentUsername() => GetCurrentIdenity().Claims.FirstOrDefault(s => s.Type == ClaimTypes.Name).Value;

        public Guid GetAccountId() => Guid.Parse(GetCurrentIdenity().Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier).Value);
    }
}
