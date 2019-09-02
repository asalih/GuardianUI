using Guardian.Infrastructure.Entity;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface ITargetRepository : IRepository<Target>
    {
        Task<bool> AnyTargetWithTheDomain(string domain);
    }
}
