using Guardian.Infrastructure.Entity;
using System;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.Specs
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByEmailAddress(string email);

        Task<Account> GetByToken(Guid id, string token);
    }
}
