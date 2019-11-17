using System.Threading.Tasks;
using Guardian.Domain.Account;
using Guardian.Infrastructure.Security.Specs;
using Guardian.Web.UI.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Guardian.Domain.Account.Login;
using static Guardian.Domain.Account.SignUp;

namespace Guardian.Web.UI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IIdentityHelper _identityHelper;

        public AccountController(IMediator mediator, IIdentityHelper identityHelper)
        {
            _mediator = mediator;
            _identityHelper = identityHelper;
        }

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/");
        }

        public ActionResult Forbidden()
        {
            return Content("Forbidden!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AccountLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var account = await _mediator.Send(new Login.Command()
            {
                Account = model
            });

            if (!account.IsSucceeded)
            {
                Alert(AlertTypes.Error, "Incorrect email or password!");
                return View(model);
            }

            return RedirectToAction(nameof(TargetsController.Index), "Targets");
        }

        // GET: Account/Create
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(AccountSignUpDto model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var account = await _mediator.Send(new SignUp.Command()
            {
                Account = model
            });

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        public async Task<ActionResult> Information()
        {
            var accountId = _identityHelper.GetAccountId();

            if (!accountId.HasValue)
            {
                return NotFound();
            }

            var query = await _mediator.Send(new Details.Query(accountId.Value));

            if (query?.Result == null ||
                query?.IsSucceeded != true)
            {
                return NotFound();
            }

            return View(query.Result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Information(AccountDto model)
        {
            var accountId = _identityHelper.GetAccountId();

            if (!accountId.HasValue)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = accountId.Value;
            var result = await _mediator.Send(new Update.Command()
            {
                Account = model
            });

            if (!result.IsSucceeded)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Information));
        }
    }
}