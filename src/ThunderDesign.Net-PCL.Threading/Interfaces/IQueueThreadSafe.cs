using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net_PCL.Threading.Interfaces
{
    public interface IQueueThreadSafe : IEnumerable, ICollection
    {
    }

    public interface IQueueThreadSafe<T> : IEnumerable<T>, IReadOnlyCollection<T>, IQueueThreadSafe
    {
    }
}
