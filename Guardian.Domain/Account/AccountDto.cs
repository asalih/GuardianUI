using Guardian.Infrastructure.Entity.Specs;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Guardian.Domain.Account
{
    public class AccountDto : DtoBase, IAccount
    {
        public string Email { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public string Password { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        [Display(Name = "Password Again")]
        public string PasswordAgain { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public string Salt { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string Token { get; set; }

        [JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [IgnoreDataMember]
        public bool ReGenerateToken { get; set; }
    }
}
