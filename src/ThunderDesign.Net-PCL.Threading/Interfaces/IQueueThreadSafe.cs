using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IQueueThreadSafe : IEnumerable, ICollection
    {
    }

    public interface IQueueThreadSafe<T> : IEnumerable<T>, IQueueThreadSafe
    {
    }
}
