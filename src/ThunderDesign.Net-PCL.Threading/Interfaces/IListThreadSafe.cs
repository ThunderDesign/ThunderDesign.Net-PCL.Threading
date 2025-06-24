using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IListThreadSafe : IList
    {
    }

    public interface IListThreadSafe<T> : IList<T>, IListThreadSafe
    {
    }
}
