using Guardian.Infrastructure.Entity;
using System;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface IHTTPLogRepository : IRepository<HTTPLog>
    {
        Task<DBReportResult[]> RequestQuery(DateTime queryTime, Target target);

        Task<DBReportResult[]> RequestTimeQuery(DateTime queryTime, Target target);

        Task<DBReportResult> RequestRuleRatioQuery(DateTime queryTime, Target target);
    }
}
