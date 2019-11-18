using Guardian.Domain.Account;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Guardian.Web.UI.Filters
{
    public class ApiAuthorize : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasAuthHeader = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var value);

            if (!hasAuthHeader || string.IsNullOrWhiteSpace(value))
            {
                context.Result = new ForbidResult();

                return;
            }

            var mediator = (IMediator)context.HttpContext.RequestServices.GetService(typeof(IMediator));

            var authHeaderValues = Encoding.UTF8.GetString(Convert.FromBase64String(value.ToString())).Split(new[] { ':' }, 2);

            var result = await mediator.Send(new TokenLogin.Command()
            {
                Id = Guid.Parse(authHeaderValues[0]),
                Token = authHeaderValues[1]
            });

            if (!result.IsSucceeded ||
                result.Result == null)
            {
                context.Result = new ForbidResult();

                return;
            }
        }
    }
}
