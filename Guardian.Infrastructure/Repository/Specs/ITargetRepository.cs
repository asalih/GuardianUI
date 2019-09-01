using Guardian.Infrastructure.Entity;
using System;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface ITargetRepository : IRepository<Target>
    {
        Task<Target> GetByDomain(string domain);
    }
}
