using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.EF
{
    public class GenericRepository<T> : IRepository<T> where T : EntityBase
    {
        protected GuardianDataContext Context;

        public GenericRepository(GuardianDataContext context)
        {
            Context = context;
        }

        public async Task<T> GetById(int id) => await Context.Set<T>().FindAsync(id);

        public IQueryable<T> Query() => Context.Set<T>().AsQueryable<T>();

        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
            => await Context.Set<T>().FirstOrDefaultAsync(predicate);

        public async Task Add(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public async Task<int> Update(T entity)
        {
            // In case AsNoTracking is used
            Context.Entry(entity).State = EntityState.Modified;
            return await Context.SaveChangesAsync();
        }

        public async Task<int> Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
            return await Context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<int> CountAll() => await Context.Set<T>().CountAsync();

        public async Task<int> CountWhere(Expression<Func<T, bool>> predicate)
            => await Context.Set<T>().CountAsync(predicate);
    }
}
