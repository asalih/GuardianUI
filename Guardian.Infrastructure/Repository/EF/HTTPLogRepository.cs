using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Guardian.Infrastructure.Repository.EF
{
    public class HTTPLogRepository : BaseRepository<HTTPLog>, IHTTPLogRepository
    {
        public HTTPLogRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {

        }

        public override Task Add(HTTPLog entity)
        {
            throw new NotImplementedException();
        }

        public override async Task<HTTPLog> FirstOrDefault(Expression<Func<HTTPLog, bool>> predicate) =>
            await DbSet.Where(s => s.Target.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<HTTPLog> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id && s.Target.AccountId == AccountId);

        public IQueryable<HTTPLog> Query() => DbSet.AsQueryable<HTTPLog>().Where(s => s.Target.AccountId == AccountId);

        public async Task<DBReportResult[]> RequestQuery(DateTime queryTime, Target target)
        {
            var commandText = "SELECT date_trunc('min', s0.\"CreatedAt\") \"min\", COUNT(1) " +
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

        public async Task<DBReportResult[]> RequestTimeQuery(DateTime queryTime, Target target)
        {
            var commandText = "SELECT date_trunc('min', s0.\"CreatedAt\") \"min\", AVG(\"HttpElapsed\") " +
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

        public async Task<DBReportResult> RequestRuleRatioQuery(DateTime queryTime, Target target)
        {
            var commandText = "SELECT SUM(\"HttpElapsed\" + \"RuleCheckElapsed\") / SUM(\"RuleCheckElapsed\") \"Ratio\" " +
"FROM \"HTTPLogs\" AS s0 " +
"INNER JOIN \"Targets\" AS \"s.Target0\" ON s0.\"TargetId\" = \"s.Target0\".\"Id\" " +
"WHERE (\"s.Target0\".\"AccountId\" = @AccountId) " +
"AND (s0.\"TargetId\" = @TargetId) " +
"AND (s0.\"CreatedAt\" >= @CreatedAt)";

            using (var command = Context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = commandText;
                command.Parameters.Add(new NpgsqlParameter("@AccountId", AccountId));
                command.Parameters.Add(new NpgsqlParameter("@TargetId", target.Id));
                command.Parameters.Add(new NpgsqlParameter("@CreatedAt", queryTime));

                Context.Database.OpenConnection();
                using (var result = await command.ExecuteReaderAsync())
                {
                    if (await result.ReadAsync())
                    {
                        return new DBReportResult()
                        {
                            Time = queryTime,
                            Value = result.GetDouble(0).ToString("0.##")
                        };
                    }
                }

                return null;
            }
        }
    }
}
