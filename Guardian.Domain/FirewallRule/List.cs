using AutoMapper;
using AutoMapper.QueryableExtensions;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.FirewallRule
{
    public class List
    {
        public class Query : IRequest<QueryListResult<FirewallRuleDto>>
        {
            public Query(Guid? targetId, int? limit = null, int? offset = null)
            {
                Limit = limit;
                Offset = offset;
                TargetId = targetId;
            }

            public int? Limit { get; }
            public int? Offset { get; }
            public Guid? TargetId { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryListResult<FirewallRuleDto>>
        {
            private readonly IFirewallRuleRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IFirewallRuleRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<QueryListResult<FirewallRuleDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var query = _repository.Query();

                var firewallRules = await query
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .ProjectTo<FirewallRuleDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new QueryListResult<FirewallRuleDto>()
                {
                    Result = firewallRules,
                    Count = query.Count(),
                    IsSucceeded = true
                };
            }
        }
    }
}
