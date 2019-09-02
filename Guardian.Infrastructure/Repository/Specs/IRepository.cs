using Guardian.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<T> GetById(Guid id);
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);

        IQueryable<T> Query();

        Task Add(T entity);
        Task<int> Update(T entity);
        Task<int> Remove(T entity);
    }
}
