using Guardian.Infrastructure.Entity.Specs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Guardian.Infrastructure.Entity
{
    public class Account : EntityBase, IAccount
    {
        [StringLength(200)]
        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        [StringLength(50)]
        public string FullName { get; set; }

        public ICollection<Target> Targets { get; set; }
    }
}
