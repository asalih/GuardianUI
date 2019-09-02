using Guardian.Infrastructure.Data;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Security.Specs;
using Microsoft.EntityFrameworkCore;
using System;

namespace Guardian.Infrastructure.Repository.EF
{
    public class BaseRepository<T> where T : EntityBase
    {
        protected GuardianDataContext Context { get; }
        protected IIdentityHelper IdentityHelper { get; }
        protected Guid? AccountId { get; }
        protected DbSet<T> DbSet { get; }

        public BaseRepository(GuardianDataContext context, IIdentityHelper identityHelper)
        {
            Context = context;

            DbSet = context.Set<T>();

            AccountId = identityHelper.GetAccountId();
            IdentityHelper = identityHelper;
        }
    }
}
