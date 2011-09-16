using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceHandlerLib.Collections
{
    public class FilteringCollection<T, M> : AdaptingCollection<T, M>
    {
        public FilteringCollection(Func<Lazy<T, M>, bool> filter)
            : base(e => e.Where(filter))
        { }
    }
}
