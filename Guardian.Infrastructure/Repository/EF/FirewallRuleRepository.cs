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
    public class FirewallRuleRepository : BaseRepository<FirewallRule>, IFirewallRuleRepository
    {
        public FirewallRuleRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {

        }

        public override async Task Add(FirewallRule entity)
        {
            entity.AccountId = AccountId.Value;

            await base.Add(entity);
        }

        public override async Task<FirewallRule> FirstOrDefault(Expression<Func<FirewallRule, bool>> predicate)
            => await DbSet.Where(s => s.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<FirewallRule> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id && s.AccountId == AccountId);

        public IQueryable<FirewallRule> Query() => DbSet.AsQueryable<FirewallRule>().Where(s => s.AccountId == AccountId);
    }
}
