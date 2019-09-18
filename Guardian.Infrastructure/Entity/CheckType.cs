using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian.Infrastructure.Entity
{
    public enum CheckType
    {
        Contains,
        StartsWith,
        EndWith,
        NotContains,
        Regex
    }
}
