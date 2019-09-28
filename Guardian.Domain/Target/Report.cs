using AutoMapper;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                ReportType = type;
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

                return await HandleReportQuery(message, target, cancellationToken);
            }

            private async Task<QueryListResult<TargetReportDto>> HandleReportQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                DBReportResult[] qResult = null;

                switch (message.ReportType)
                {
                    case ReportType.Request:
                        qResult = await _logRepository.RequestQuery(dt, target);
                        break;
                    case ReportType.RequestTime:
                        qResult = await _logRepository.RequestTimeQuery(dt, target);
                        break;
                    case ReportType.Rule:
                        qResult = await _ruleLogRepository.RuleQuery(dt, target);
                        break;
                    case ReportType.RuleTime:
                        qResult = await _ruleLogRepository.RuleTimeQuery(dt, target);
                        break;
                    case ReportType.RequestRuleRatio:
                        var ratioResult = await _logRepository.RequestRuleRatioQuery(dt, target);

                        return new QueryListResult<TargetReportDto>()
                        {
                            Result = new List<TargetReportDto>()
                            {
                                new TargetReportDto()
                                {
                                    Time = ratioResult.Time.ToShortTimeString(),
                                    Value = ratioResult.Value
                                }
                            },
                            IsSucceeded = true
                        };
                    default:
                        break;
                }

                var returnResult = new List<TargetReportDto>(30);
                for (int i = 0; i < 31; i++)
                {
                    var refDate = dt.AddMinutes(i);

                    returnResult.Add(new TargetReportDto()
                    {
                        Time = refDate.ToShortTimeString(),
                        Value = qResult.FirstOrDefault(s => s.Time.Minute == refDate.Minute)?.Value ?? 0
                    });
                }

                return new QueryListResult<TargetReportDto>()
                {
                    Result = returnResult,
                    IsSucceeded = true
                };
            }
        }
    }
}
