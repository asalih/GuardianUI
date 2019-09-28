using Guardian.Domain.FirewallRule;
using Guardian.Web.UI.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Guardian.Web.UI.Controllers
{
    [Authorize]
    public class FirewallRulesController : BaseController
    {
        private readonly IMediator _mediator;

        public FirewallRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> Index([FromRoute]Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index", "Targets");
            }

            //TODO: Add paging.

            var result = await _mediator.Send(new List.Query(id));

            ViewBag.TargetId = id;

            return View(result);
        }

        [HttpGet]
        public ActionResult Create(Guid id) => View(new FirewallRuleDto
        {
            TargetId = id,
            IsActive = true
        });

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var query = await _mediator.Send(new Details.Query(id));

            if (query?.Result == null ||
                query?.IsSucceeded != true)
            {
                return NotFound();
            }

            return View(query.Result);
        }

        [HttpPost]
        public async Task<ActionResult> Create(FirewallRuleDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new Add.Command()
            {
                FirewallRule = model
            });

            if (!result.IsSucceeded)
            {
                return View(model);
            }

            Alert(AlertTypes.Success, "Firewall Rule successfully added!");

            return RedirectToAction(nameof(Index), new { id = model.TargetId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(FirewallRuleDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new Update.Command()
            {
                FirewallRule = model
            });

            if (!result.IsSucceeded)
            {
                return View(model);
            }

            Alert(AlertTypes.Success, "Firewall Rule successfully updated!");

            return RedirectToAction(nameof(Index), new { id = model.TargetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid ruleId)
        {
            var result = await _mediator.Send(new Delete.Command()
            {
                Id = ruleId
            });

            if (result.IsSucceeded)
            {
                Alert(AlertTypes.Success, "Firewall Rule successfully deleted!");
            }
            else
            {
                Alert(AlertTypes.Error, "Firewall Rule couldn't deleted!");
            }

            if(result.Result?.TargetId == null)
            {
                return RedirectToAction(nameof(Index), "Targets");
            }

            return RedirectToAction(nameof(Index), new { id = result.Result.TargetId });
        }
    }
}
