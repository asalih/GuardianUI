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
    public class WafRuleRepository : BaseRepository<WafRule>, IWafRuleRepository
    {
        public WafRuleRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {

        }

        public override async Task Add(WafRule entity)
        {
            entity.AccountId = AccountId.Value;

            await base.Add(entity);
        }

        public async Task<WafRule> FirstOrDefault(Expression<Func<WafRule, bool>> predicate)
            => await DbSet.Where(s => s.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<WafRule> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id && s.AccountId == AccountId);

        public IQueryable<WafRule> Query() => DbSet.AsQueryable<WafRule>().Where(s => s.AccountId == AccountId);
    }
}
