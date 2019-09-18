using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.EF
{
    public class BaseRepository<T> where T : EntityBase
    {
        protected GuardianDataContext Context { get; }
        protected IIdentityHelper IdentityHelper { get; }
        protected Guid? AccountId { get; }
        protected DbSet<T> DbSet { get; }

        public BaseRepository(GuardianDataContext context, IIdentityHelper identityHelper)
        {
            Context = context;

            DbSet = context.Set<T>();

            AccountId = identityHelper.GetAccountId();
            IdentityHelper = identityHelper;
        }

        public virtual async Task Add(T entity)
        {
            await DbSet.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async virtual Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
            => await DbSet.FirstOrDefaultAsync(predicate);

        public async Task<int> Remove(T entity)
        {
            DbSet.Remove(entity);

            return await Context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            // In case AsNoTracking is used
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync();
        }
    }
}
