using AutoMapper;
using Guardian.Domain.FirewallRule.Serialzation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Newtonsoft.Json;
using System.Linq;
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
            private readonly IParser _parser;

            public QueryHandler(IFirewallRuleRepository repository, IMapper mapper, IParser parser)
            {
                _repository = repository;
                _mapper = mapper;
                _parser = parser;
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

                var expr = _parser.GetRules(firewallRule.Expression);
                
                if (expr.Item2 && expr.Item1?.Any() == true)
                {
                    foreach (var item in expr.Item1)
                    {
                        if (item.Action == null)
                        {
                            continue;
                        }

                        item.Action.Phase = firewallRule.RuleFor == Infrastructure.Entity.RuleFor.Request ? 1 : 3;
                    }
                }
                else
                {
                    return new CommandResult<FirewallRuleDto>()
                    {
                        IsSucceeded = false,
                        Message = "Can't parse given rule!",
                        Result = _mapper.Map<FirewallRuleDto>(firewallRule)
                    };
                }

                firewallRule.SerializedExpression = JsonConvert.SerializeObject(expr);

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
