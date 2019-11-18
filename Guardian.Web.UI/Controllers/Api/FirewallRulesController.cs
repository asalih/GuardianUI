using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guardian.Domain;
using Guardian.Domain.FirewallRule;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guardian.Web.UI.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirewallRulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FirewallRulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<QueryListResult<FirewallRuleDto>> List(Guid? targetId, int? limit, int? offset)
        {
            if (!targetId.HasValue ||
                targetId == Guid.Empty)
            {
                return new QueryListResult<FirewallRuleDto>()
                {
                    IsSucceeded = false
                };
            }

            return await _mediator.Send(new List.Query(targetId, limit, offset));
        }

        [HttpPost]
        public async Task<CommandResult<FirewallRuleDto>> Post([FromBody]FirewallRuleApiModel firewallRule)
        {
            if (!ModelState.IsValid)
            {
                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = false,
                    Message = "Validation problem. Check your values"
                };
            }

            return await _mediator.Send(new Add.Command() { FirewallRule = firewallRule });
        }

        [HttpPut("{id}")]
        public async Task<CommandResult<FirewallRuleDto>> Put([FromBody]FirewallRuleDto firewallRule)
        {
            if (!ModelState.IsValid)
            {
                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = false,
                    Message = "Validation problem. Check your values"
                };
            }

            return await _mediator.Send(new Update.Command() { FirewallRule = firewallRule });
        }

        [HttpDelete("{id}")]
        public async Task<CommandResult<FirewallRuleDto>> Delete(Guid? id)
        {
            if (!id.HasValue ||
                id == Guid.Empty)
            {
                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = false,
                    Message = "Invalid ID."
                };
            }

            return await _mediator.Send(new Delete.Command() { Id = id.Value });
        }
    }
}