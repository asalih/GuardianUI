using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using System;
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
            private readonly IIdentityHelper _identityHelper;

            public QueryHandler(IFirewallRuleRepository repository,
                IMapper mapper,
                IIdentityHelper identityHelper,
                ITargetRepository targetRepository,
                IRuleLogRepository logRepo)
            {
                _repository = repository;
                _mapper = mapper;
                _identityHelper = identityHelper;
                _targetRepository = targetRepository;
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
