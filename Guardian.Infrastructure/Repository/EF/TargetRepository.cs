using System;
using System.Threading.Tasks;
using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;

namespace Guardian.Infrastructure.Repository.EF
{
    public class TargetRepository : GenericRepository<Target>, ITargetRepository
    {
        public TargetRepository(GuardianDataContext context) : base(context)
        {
        }

        public async Task<Target> GetByDomain(string domain) => await FirstOrDefault(s => s.Domain == domain);
    }
}
