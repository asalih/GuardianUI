using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public class Operator
    {
        public string Func { get; set; }
        public string Expression { get; set; }
        public bool OperatorIsNotType { get; set; }
    }
}
