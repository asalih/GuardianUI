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

        [HttpGet]
        public IActionResult Create() => View();

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
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

        [HttpGet]
        public async Task<IActionResult> Report(Guid id, ReportType type)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var report = await _mediator.Send(new Report.Query(id, type));

            return Json(report);
        }

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReCreateCertificate(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var result = await _mediator.Send(new Update.Command()
            {
                TargetId = id,
                IsReCreateCertificateCommand = true
            });

            if (result.IsSucceeded)
            {
                Alert(AlertTypes.Success, "Successfully re-created certificate.");
            }
            else
            {
                Alert(AlertTypes.Error, "Re-creating certificate has faild.");
            }

            return RedirectToAction("Index");
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
                if (!string.IsNullOrEmpty(result.Message))
                {
                    Alert(AlertTypes.Error, result.Message);
                }

                return View(model);
            }

            Alert(AlertTypes.Success, "Target successfully added!");

            return RedirectToAction(nameof(Details), new { id = result.Result.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(TargetDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _mediator.Send(new Update.Command()
            {
                Target = model
            });

            if (!result.IsSucceeded)
            {
                return View(model);
            }

            Alert(AlertTypes.Success, "Target successfully updated!");

            return RedirectToAction(nameof(Details), new { id = result.Result.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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