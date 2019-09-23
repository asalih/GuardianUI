using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.HTTPLog
{
    public class List
    {
        public class Query : IRequest<QueryListResult<HTTPLogDto>>
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

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                DefaultValidatorExtensions.NotNull(RuleFor(x => x.TargetId)).NotNull().NotEmpty();
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryListResult<HTTPLogDto>>
        {
            private readonly IRuleLogRepository _repository;
            private readonly IMapper _mapper;

            public QueryHandler(IRuleLogRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<QueryListResult<HTTPLogDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var query = _repository.Query().Where(s => s.TargetId == message.TargetId);

                var httpLogs = await query
                    .Skip(message.Offset ?? 0)
                    .Take(message.Limit ?? 20)
                    .ProjectTo<HTTPLogDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);

                return new QueryListResult<HTTPLogDto>()
                {
                    Result = httpLogs,
                    Count = query.Count(),
                    IsSucceeded = true
                };
            }
        }
    }
}
