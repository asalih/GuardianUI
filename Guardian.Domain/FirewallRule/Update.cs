using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.FirewallRule
{
    public class Update
    {
        public class Command : IRequest<CommandResult<FirewallRuleDto>>
        {
            public FirewallRuleDto FirewallRule { get; set; }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<FirewallRuleDto>>
        {
            private readonly IFirewallRuleRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IFirewallRuleRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<FirewallRuleDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var firewallRule = await _repository.GetById(message.FirewallRule.Id);

                if (firewallRule == null)
                {
                    return new CommandResult<FirewallRuleDto>()
                    {
                        IsSucceeded = false
                    };
                }

                firewallRule.Expression = message.FirewallRule.Expression;
                firewallRule.IsActive = message.FirewallRule.IsActive;
                firewallRule.Title = message.FirewallRule.Title;

                await _repository.Update(firewallRule);

                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = true,
                    Result = _mapper.Map<FirewallRuleDto>(firewallRule)
                };
            }
        }
    }
}
