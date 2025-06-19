using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace ThunderDesign.Net_PCL.Threading.Interfaces
{
#if NET8_0_OR_GREATER
    public interface ISortedDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary, IReadOnlyDictionary<TKey, TValue>

#elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public interface ISortedDictionaryThreadSafe<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, ICollection, IDictionary
#else
    public interface ISortedDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>
#endif
    {
        #region properties
        IComparer<TKey> Comparer { get; }
        new int Count { get; }
        new ICollection<TKey> Keys { get; }
        new ICollection<TValue> Values { get; }
        new TValue this[TKey key] { get; set; }
        #endregion

        #region methods
        new void Add(TKey key, TValue value);
        new void Clear();
        new bool ContainsKey(TKey key);
        bool ContainsValue(TValue value);
        new bool Remove(TKey key);
        new bool TryGetValue(TKey key, out TValue value);
        #endregion
    }
}