using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net_PCL.Threading.Interfaces
{
    public interface IListThreadSafe : IList
    {
    }

    public interface IListThreadSafe<T> : IList<T>, IReadOnlyList<T>, IListThreadSafe
    {
    }
}
