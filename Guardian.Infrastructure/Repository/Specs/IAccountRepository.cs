using Guardian.Infrastructure.Entity;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByEmailAddress(string email);
    }
}
