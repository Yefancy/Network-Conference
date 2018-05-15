using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCLib
{
    public class NCList<T> : List<T>
    {
        public event Action<T> ItemAdded;
        public event Action<T> ItemRemoved;

        public NCList()
        {
            ItemAdded += a => { base.Add(a); };
            ItemRemoved += a => { base.Remove(a); };
        }

        public new void Add(T add)
        {
            ItemAdded(add);
        }

        public new void Remove(T remove)
        {
            ItemRemoved(remove);
        }
    }
}
