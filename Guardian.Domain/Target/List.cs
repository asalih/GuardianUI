using AutoMapper;
using AutoMapper.QueryableExtensions;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Guardian.Infrastructure.Security.Specs;

namespace Guardian.Domain.Target
{
    public class List
    {
        public class Query : IRequest<QueryListResult<TargetDto>>
        {
            public Query(int? limit = null, int? offset = null)
            {
                Limit = limit;
                Offset = offset;
            }

            public int? Limit { get; }
            public int? Offset { get; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryListResult<TargetDto>>
        {
            private readonly ITargetRepository _repository;
            private readonly IMapper _mapper;
            private readonly IIdentityHelper _identityHelper;

            public QueryHandler(ITargetRepository repository,
                IMapper mapper,
                IIdentityHelper identityHelper)
            {
                _repository = repository;
                _mapper = mapper;
                _identityHelper = identityHelper;
            }

            public async Task<QueryListResult<TargetDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var accountId = _identityHelper.GetAccountId();

                var query = _repository.Query()
                    .Where(s => s.AccountId == accountId);

                var targets = await query
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .ProjectTo<TargetDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new QueryListResult<TargetDto>()
                {
                    Result = targets,
                    Count = query.Count(),
                    IsSucceeded = true
                };
            }
        }
    }
}
