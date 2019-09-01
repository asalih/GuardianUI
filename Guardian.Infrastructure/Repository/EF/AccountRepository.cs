using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Repository.Specs;
using System.Threading.Tasks;

namespace Guardian.Infrastructure.Repository.EF
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(GuardianDataContext context) : base(context)
        {

        }

        public async Task<Account> GetByEmailAddress(string email) => await FirstOrDefault(s => s.Email == email);
    }
}
