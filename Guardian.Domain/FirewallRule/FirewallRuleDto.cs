using System;
using System.ComponentModel.DataAnnotations;
using Guardian.Domain.Account;
using Guardian.Domain.Target;
using Guardian.Infrastructure.Entity.Specs;

namespace Guardian.Domain.FirewallRule
{
    public class FirewallRuleDto : DtoBase, IFirewallRule
    {
        public Guid AccountId { get; set; }
        public AccountDto Account { get; set; }
        public Guid TargetId { get; set; }
        public TargetDto Target { get; set; }
        public string Title { get; set; }
        public string Expression { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
