using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.EF
{
    public class RuleLogRepository : BaseRepository<RuleLog>, IRuleLogRepository
    {
        public RuleLogRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {
            
        }

        public override Task Add(RuleLog entity)
        {
            throw new NotImplementedException();
        }

        public override async Task<RuleLog> FirstOrDefault(Expression<Func<RuleLog, bool>> predicate)
            => await DbSet.Where(s => s.Target.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<RuleLog> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id && s.Target.AccountId == AccountId);

        public IQueryable<RuleLog> Query() => DbSet.AsQueryable<RuleLog>().Where(s => s.Target.AccountId == AccountId);

        public async Task<DBReportResult[]> RuleQuery(DateTime queryTime, Target target)
        {
            var commandText = "SELECT date_trunc('min', s0.\"CreatedAt\") \"min\", Count(1) " +
"FROM \"RuleLogs\" AS s0 " +
"INNER JOIN \"Targets\" AS \"s.Target0\" ON s0.\"TargetId\" = \"s.Target0\".\"Id\" " +
"WHERE (\"s.Target0\".\"AccountId\" = @AccountId) " +
"AND (s0.\"TargetId\" = @TargetId) " +
"AND (s0.\"CreatedAt\" >= @CreatedAt) " +
"group by 1";

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.Add(new NpgsqlParameter("@AccountId", AccountId));
                command.Parameters.Add(new NpgsqlParameter("@TargetId", target.Id));
                command.Parameters.Add(new NpgsqlParameter("@CreatedAt", queryTime));

                var logs = new List<DBReportResult>();

                Context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        logs.Add(new DBReportResult()
                        {
                            Time = result.GetDateTime(0),
                            Value = Convert.ToInt32(result.GetDouble(1))
                        });
                    }
                }

                return logs.ToArray();
            }
        }

        public async Task<DBReportResult[]> RuleTimeQuery(DateTime queryTime, Target target)
        {
            var commandText = "SELECT date_trunc('min', s0.\"CreatedAt\") \"min\", Avg(s0.\"RuleCheckElapsed\") " +
"FROM \"HTTPLogs\" AS s0 " +
"INNER JOIN \"Targets\" AS \"s.Target0\" ON s0.\"TargetId\" = \"s.Target0\".\"Id\" " +
"WHERE (\"s.Target0\".\"AccountId\" = @AccountId) " +
"AND (s0.\"TargetId\" = @TargetId) " +
"AND (s0.\"CreatedAt\" >= @CreatedAt) " +
"group by 1";

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.Add(new NpgsqlParameter("@AccountId", AccountId));
                command.Parameters.Add(new NpgsqlParameter("@TargetId", target.Id));
                command.Parameters.Add(new NpgsqlParameter("@CreatedAt", queryTime));

                var logs = new List<DBReportResult>();

                Context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    while (await result.ReadAsync())
                    {
                        logs.Add(new DBReportResult()
                        {
                            Time = result.GetDateTime(0),
                            Value = Convert.ToInt32(result.GetDouble(1))
                        });
                    }
                }

                return logs.ToArray();
            }
        }
    }
}
