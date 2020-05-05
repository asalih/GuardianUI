using System.Collections.Generic;

namespace Guardian.Domain.FirewallRule.Serialzation
{
    public interface IParser
    {
        (List<Rule>, bool) GetRules(string raw);
    }
}
