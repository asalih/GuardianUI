using AutoMapper;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Guardian.Domain.Target
{
    public class Report
    {
        public class Query : IRequest<QueryListResult<TargetReportDto>>
        {
            public Query(Guid id, ReportType type)
            {
                TargetId = id;
            }

            public Guid TargetId { get; }
            public ReportType ReportType { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, QueryListResult<TargetReportDto>>
        {
            private readonly ITargetRepository _repository;
            private readonly IHTTPLogRepository _logRepository;
            private readonly IRuleLogRepository _ruleLogRepository;
            private readonly IMapper _mapper;

            public QueryHandler(ITargetRepository repository,
                IHTTPLogRepository logRepository,
                IRuleLogRepository ruleLogRepository,
                IMapper mapper)
            {
                _repository = repository;
                _logRepository = logRepository;
                _ruleLogRepository = ruleLogRepository;
                _mapper = mapper;
            }

            public async Task<QueryListResult<TargetReportDto>> Handle(Query message, CancellationToken cancellationToken)
            {
                var target = await _repository.GetById(message.TargetId);

                if (target == null)
                {
                    return new QueryListResult<TargetReportDto>()
                    {
                        IsSucceeded = false
                    };
                }

                switch (message.ReportType)
                {
                    case ReportType.Request:
                        return await HandleRequestQuery(message, target, cancellationToken);
                    case ReportType.Rule:
                        return await HandleRuleQuery(message, target, cancellationToken);
                    default:
                        break;
                }

                return null;

            }

            private async Task<QueryListResult<TargetReportDto>> HandleRequestQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                var query = from log in _logRepository.Query()
                              where log.TargetId == target.Id && log.CreatedAt >= dt
                              group log by log.CreatedAt.Minute into g
                              select new { Minute = g.Key, Count = g.Count() };

                var results = await query.ToListAsync(cancellationToken);

                return new QueryListResult<TargetReportDto>()
                {
                    Result = results.Select(s => new TargetReportDto
                    {
                        DateTime = dt.AddMinutes(s.Minute),
                        Value = s.Count
                    }).ToList(),
                    IsSucceeded = true
                };
            }

            private async Task<QueryListResult<TargetReportDto>> HandleRuleQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                var query = from log in _ruleLogRepository.Query()
                            where log.TargetId == target.Id && log.CreatedAt >= dt
                            group log by log.CreatedAt.Minute into g
                            select new { Minute = g.Key, Count = g.Count() };

                var results = await query.ToListAsync(cancellationToken);

                return new QueryListResult<TargetReportDto>()
                {
                    Result = results.Select(s => new TargetReportDto
                    {
                        DateTime = dt.AddMinutes(s.Minute),
                        Value = s.Count
                    }).ToList(),
                    IsSucceeded = true
                };
            }
        }
    }
}
