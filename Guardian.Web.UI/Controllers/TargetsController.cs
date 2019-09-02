using System;
using System.Threading.Tasks;
using Guardian.Domain;
using Guardian.Domain.Target;
using Guardian.Web.UI.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Guardian.Web.UI.Controllers
{
    [Authorize]
    public class TargetsController : BaseController
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
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new Delete.Command()
            {
                Target = new TargetDto()
                {
                    Id = id
                }
            });

            if (result.IsSucceeded)
            {
                Alert(AlertTypes.Success, "Target successfully deleted!");
            }
            else
            {
                Alert(AlertTypes.Error, "Target couldn't deleted!");
            }

            return RedirectToAction(nameof(Index));
        }
    }
}