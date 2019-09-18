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
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(GuardianDataContext context, IIdentityHelper identityHelper)
            : base(context, identityHelper)
        {

        }

        public async Task<Account> FirstOrDefault(Expression<Func<Account, bool>> predicate)
            => await DbSet.FirstOrDefaultAsync(predicate);

        public async Task<Account> GetByEmailAddress(string email) => await FirstOrDefault(s => s.Email == email);

        public async Task<Account> GetById(Guid id) => await DbSet.FirstOrDefaultAsync(s => s.Id == id);

        public IQueryable<Account> Query() => DbSet.AsQueryable<Account>();
    }
}
