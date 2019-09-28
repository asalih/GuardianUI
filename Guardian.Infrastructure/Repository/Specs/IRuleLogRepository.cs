using Guardian.Infrastructure.Entity;
using System;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface IRuleLogRepository : IRepository<RuleLog>
    {
        Task<DBReportResult[]> RuleQuery(DateTime queryTime, Target target);

        Task<DBReportResult[]> RuleTimeQuery(DateTime queryTime, Target target);
    }
}
