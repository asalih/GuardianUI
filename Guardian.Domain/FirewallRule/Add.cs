using AutoMapper;
using Guardian.Domain.FirewallRule.Serialzation;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.FirewallRule
{
    public class Add
    {
        public class Command : IRequest<CommandResult<FirewallRuleDto>>
        {
            public FirewallRuleDto FirewallRule { get; set; }
        }

        public class QueryHandler : IRequestHandler<Command, CommandResult<FirewallRuleDto>>
        {
            private readonly IFirewallRuleRepository _repository;
            private readonly ITargetRepository _targetRepository;
            private readonly IMapper _mapper;
            private readonly IParser _parser;

            public QueryHandler(IFirewallRuleRepository repository,
                IMapper mapper,
                ITargetRepository targetRepository,
                IParser parser)
            {
                _repository = repository;
                _mapper = mapper;
                _targetRepository = targetRepository;
                _parser = parser;
            }

            public async Task<CommandResult<FirewallRuleDto>> Handle(Command message, CancellationToken cancellationToken)
            {
                var target = await _targetRepository.FirstOrDefault(s => s.Id == message.FirewallRule.TargetId);

                if (target == null)
                {
                    return new CommandResult<FirewallRuleDto>()
                    {
                        IsSucceeded = true,
                        Message = "Target not found!"
                    };
                }

                message.FirewallRule.Id = Guid.NewGuid();
                message.FirewallRule.CreatedAt = DateTime.UtcNow;

                var firewallRule = _mapper.Map<Infrastructure.Entity.FirewallRule>(message.FirewallRule);

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

                await _repository.Add(firewallRule);

                return new CommandResult<FirewallRuleDto>()
                {
                    IsSucceeded = true,
                    Message = "Firewall rule successfully created!",
                    Result = _mapper.Map<FirewallRuleDto>(firewallRule)
                };

            }
        }
    }
}
