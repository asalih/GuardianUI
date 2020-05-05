using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Guardian.Domain.Account;
using Guardian.Domain.Target;
using Guardian.Infrastructure.Entity;
using Guardian.Infrastructure.Entity.Specs;

namespace Guardian.Domain.FirewallRule
{
    public class FirewallRuleDto : DtoBase, IFirewallRule
    {
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public Guid AccountId { get; set; }
        
        public Guid TargetId { get; set; }
        
        public string Title { get; set; }
        
        public string Expression { get; set; }

        [Display(Name = "Rule For")]
        public RuleFor RuleFor { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }

    public class FirewallRuleApiModel : FirewallRuleDto
    {
        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public new Guid Id { get; set; }
    }
}
