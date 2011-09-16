using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceHandlerLib.Collections
{
    public class OrderingCollection<T, M> : AdaptingCollection<T, M>
    {
        public OrderingCollection(Func<Lazy<T, M>, object> keySelector, bool descending = false)
            : base(e => descending ? e.OrderByDescending(keySelector) : e.OrderBy(keySelector))
        { }

    }
}
