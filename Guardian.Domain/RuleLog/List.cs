using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.WafRuleLog
{
    public class List
    {
        public class Query : IRequest<QueryListResult<FirewallRuleLogDto>>
        {
            public Query(Guid? wafRuleId, int? limit = null, int? offset = null)
            {
                Limit = limit;
                Offset = offset;
                WafRuleId = wafRuleId;
            }

            public int? Limit { get; }
            public int? Offset { get; }
            public Guid? WafRuleId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.WafRuleId)).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryListResult<FirewallRuleLogDto>>
        {
            private readonly IRuleLogRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IRuleLogRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<QueryListResult<FirewallRuleLogDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var query = _repository.Query();

                var wafRuleLogs = await query
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .ProjectTo<FirewallRuleLogDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new QueryListResult<FirewallRuleLogDto>()
                {
                    Result = wafRuleLogs,
                    Count = query.Count(),
                    IsSucceeded = true
                };
            }
        }
    }
}
