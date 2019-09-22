using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;
using System;
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

        public override async Task<RuleLog> FirstOrDefault(Expression<Func<RuleLog, bool>> predicate)
            => await DbSet.Where(s => s.Target.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<RuleLog> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id && s.Target.AccountId == AccountId);

        public IQueryable<RuleLog> Query() => DbSet.AsQueryable<RuleLog>().Where(s => s.Target.AccountId == AccountId);
    }
}
