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
    }
}
