using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public class Rule
    {
        public Rule()
        {
            Variables = new List<Variable>();
            Operator = new Operator();
            Action = new Action();
        }
        public List<Variable> Variables { get; set; }
        public Operator Operator { get; set; }
        public Action Action { get; set; }
        public Rule Chain { get; set; }
    }
}
