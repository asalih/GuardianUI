using Guardian.Infrastructure.Entity.Specs;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Guardian.Domain.Account
{
    public class AccountDto : DtoBase, IAccount
    {
        public string Email { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string Password { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public string Salt { get; set; }

        public string FullName { get; set; }
    }
}
