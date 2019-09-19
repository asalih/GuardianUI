using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.FirewallRule
{
    public class Details
    {
        public class Query : IRequest<CommandResult<FirewallRuleDto>>
        {
            public Query(Guid id)
            {
                FirwallRuleId = id;
            }

            public Guid FirwallRuleId { get; }
        }

        public class QueryHandler : IRequestHandler<Query, CommandResult<FirewallRuleDto>>
        {
            private readonly IFirewallRuleRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IFirewallRuleRepository repository,
                IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<CommandResult<FirewallRuleDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var firewallRule = await _repository.Query()
                    .Include(f => f.Target)
                    .FirstOrDefaultAsync(f => f.Id == message.FirwallRuleId);

                return new CommandResult<FirewallRuleDto>()
                {
                    Result = _mapper.Map<FirewallRuleDto>(firewallRule),
                    IsSucceeded = true
                };
            }
        }
    }
}
