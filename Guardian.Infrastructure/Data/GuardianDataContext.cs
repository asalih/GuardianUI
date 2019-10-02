using Guardian.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Guardian.Infrastructure.Data
{
    public class GuardianDataContext : DbContext
    {
        private IDbContextTransaction _currentTransaction;

        public GuardianDataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Target> Targets { get; set; }

        public DbSet<FirewallRule> FirewallRules { get; set; }

        public DbSet<RuleLog> RuleLogs { get; set; }

        public DbSet<HTTPLog> HTTPLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Target>(b =>
            {
                b.HasKey(t => t.Id);

                b.HasIndex(s => new { s.Domain, s.State });

                b.HasOne(t => t.Account)
                    .WithMany(acc => acc.Targets)
                    .HasForeignKey(t => t.AccountId);
            });

            modelBuilder.Entity<Account>(b =>
            {
                b.HasKey(t => t.Id);

                b.HasMany(acc => acc.Targets)
                    .WithOne(t => t.Account)
                    .HasForeignKey(t => t.AccountId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public void BeginTransaction()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = Database.BeginTransaction(IsolationLevel.ReadUncommitted);
        }

        public void CommitTransaction()
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }
    }
}
