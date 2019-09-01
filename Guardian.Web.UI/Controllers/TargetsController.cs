using System;
using System.Threading.Tasks;
using Guardian.Domain;
using Guardian.Domain.Target;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardian.Web.UI.Controllers
{
    [Authorize]
    public class TargetsController : Controller
    {
        private readonly IMediator _mediator;

        public TargetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            //TODO: Add paging.

            var result = await _mediator.Send(new List.Query());

            return View(result);
        }

        public IActionResult Create(Guid id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TargetDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new Add.Command()
            {
                Target = model
            });

            if (!result.IsSucceeded)
            {
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}