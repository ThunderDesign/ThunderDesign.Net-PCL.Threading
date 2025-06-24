using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface ISortedListThreadSafe : IDictionary
    {
    }

    public interface ISortedListThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, ISortedListThreadSafe
    {
    }
}
