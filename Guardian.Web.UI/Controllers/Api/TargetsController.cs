using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Guardian.Domain.Target;
using Guardian.Domain;
using Guardian.Web.UI.Filters;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Guardian.Web.UI.Controllers.Api
{
    [Route("api/[controller]")]
    public class TargetsController : Controller
    {
        private readonly IMediator _mediator;

        public TargetsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<controller>
        [HttpGet]
        [ApiAuthorize]
        public async Task<QueryListResult<TargetDto>> List(int? limit, int? offset)
        {
            return await _mediator.Send(new List.Query(limit, offset));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<CommandResult<TargetDto>> Get(Guid? id)
        {
            if (!id.HasValue ||
                id == Guid.Empty)
            {
                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = false,
                    Message = "Invalid ID."
                };
            }

            return await _mediator.Send(new Details.Query(id.Value));
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<CommandResult<TargetDto>> Post([FromBody]TargetDto target)
        {
            if (!ModelState.IsValid)
            {
                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = false,
                    Message = "Validation problem. Check your values"
                };
            }

            return await _mediator.Send(new Add.Command() { Target = target });
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<CommandResult<TargetDto>> Put([FromBody]TargetDto target)
        {
            if (!ModelState.IsValid)
            {
                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = false,
                    Message = "Validation problem. Check your values"
                };
            }

            return await _mediator.Send(new Update.Command() { Target = target });
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<CommandResult<TargetDto>> Delete(Guid? id)
        {
            if (!id.HasValue ||
                id == Guid.Empty)
            {
                return new CommandResult<TargetDto>()
                {
                    IsSucceeded = false,
                    Message = "Invalid ID."
                };
            }

            return await _mediator.Send(new Delete.Command() { Target = new TargetDto() { Id = id.Value } });
        }
    }
}
