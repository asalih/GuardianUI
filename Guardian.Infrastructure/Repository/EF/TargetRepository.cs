using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;

namespace Guardian.Infrastructure.Repository.EF
{
    public class TargetRepository : BaseRepository<Target>, ITargetRepository
    {
        public TargetRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {

        }

        public override async Task Add(Target entity)
        {
            entity.AccountId = AccountId.Value;

            await base.Add(entity);
        }

        public async Task<Target> GetTargetWithTheDomain(string domain) =>
            await DbSet.FirstOrDefaultAsync(s => s.AccountId == AccountId && s.Domain == domain);

        public override async Task<Target> FirstOrDefault(Expression<Func<Target, bool>> predicate)
            => await DbSet.Where(s => s.AccountId == AccountId).FirstOrDefaultAsync(predicate);

        public async Task<Target> GetById(Guid id) => await DbSet.Include(s=>s.FirewallRules).FirstOrDefaultAsync(s => s.Id == id && s.AccountId == AccountId);

        public IQueryable<Target> Query() => DbSet.AsQueryable<Target>().Where(s => s.AccountId == AccountId);
    }
}
