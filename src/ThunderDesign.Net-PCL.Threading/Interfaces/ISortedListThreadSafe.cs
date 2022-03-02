using System.Collections;
using System.Collections.Generic;

namespace ThunderDesign.Net_PCL.Threading.Interfaces
{
    public interface ISortedListThreadSafe : IDictionary
    {
    }

    public interface ISortedListThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ISortedListThreadSafe
    {
    }
}
