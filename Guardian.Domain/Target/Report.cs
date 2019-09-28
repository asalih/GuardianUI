using AutoMapper;
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

                switch (message.ReportType)
                {
                    case ReportType.Request:
                        return await HandleRequestQuery(message, target, cancellationToken);
                    case ReportType.Rule:
                        return await HandleRuleQuery(message, target, cancellationToken);
                    case ReportType.RuleTime:
                        return await HandleRuleTimeQuery(message, target, cancellationToken);
                    default:
                        break;
                }

                return null;

            }

            private async Task<QueryListResult<TargetReportDto>> HandleRequestQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                var qResult = await _logRepository.RequestQuery(dt, target);

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

            private async Task<QueryListResult<TargetReportDto>> HandleRuleQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                var qResult = await _ruleLogRepository.RuleQuery(dt, target);

                var returnResult = new List<TargetReportDto>(30);
                for (int i = 0; i < 31; i++)
                {
                    var refDate = dt.AddMinutes(i);

                    returnResult.Add(new TargetReportDto()
                    {
                        Time = dt.AddMinutes(i).ToShortTimeString(),
                        Value = qResult.FirstOrDefault(s => s.Time.Minute == refDate.Minute)?.Value ?? 0
                    });
                }

                return new QueryListResult<TargetReportDto>()
                {
                    Result = returnResult,
                    IsSucceeded = true
                };
            }

            private async Task<QueryListResult<TargetReportDto>> HandleRuleTimeQuery(Query message, Infrastructure.Entity.Target target, CancellationToken cancellationToken)
            {
                var dt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30));

                var qResult = await _ruleLogRepository.RuleTimeQuery(dt, target);

                var returnResult = new List<TargetReportDto>(30);
                for (int i = 0; i < 31; i++)
                {
                    var refDate = dt.AddMinutes(i);

                    returnResult.Add(new TargetReportDto()
                    {
                        Time = dt.AddMinutes(i).ToShortTimeString(),
                        Value = Convert.ToInt32(qResult.FirstOrDefault(s => s.Time.Minute == refDate.Minute)?.Value ?? 0)
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
