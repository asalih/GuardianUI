using System.Collections.Generic;

namespace Guardian.Domain
{
    public class QueryListResult<T>
    {
        public IList<T> Result { get; set; }

        public int Count { get; set; }

        public bool IsSucceeded { get; set; }
    }
}
