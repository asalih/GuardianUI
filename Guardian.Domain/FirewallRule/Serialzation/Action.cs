using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public class Action
    {
        public Action()
        {
            Transformations = new List<string>();
        }
        public string ID { get; set; }
        public int Phase { get; set; }
        public List<string> Transformations { get; set; }
        public DisruptiveAction DisruptiveAction { get; set; }
        public int LogAction { get; set; }
    }
}
