using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public class Variable
    {
        public Variable()
        {
            Filter = new List<string>();
        }
        public string Name { get; set; }
        public List<string> Filter { get; set; }
        public bool FilterIsNotType { get; set; }
        public bool LengthCheckForCollection { get; set; }
    }
}
