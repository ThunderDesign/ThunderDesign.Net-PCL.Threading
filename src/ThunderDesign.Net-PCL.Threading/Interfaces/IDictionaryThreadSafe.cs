using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif
namespace ThunderDesign.Net.Threading.Interfaces
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public interface IDictionaryThreadSafe : IDictionary, IDeserializationCallback, ISerializable
#else
    public interface IDictionaryThreadSafe : IDictionary
#endif
    {
    }

    public interface IDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionaryThreadSafe
    {
        new TValue this[TKey key] { get; set; }
        new ICollection<TKey> Keys { get; }
        new ICollection<TValue> Values { get; }
        new int Count { get; }
        new bool IsReadOnly { get; }
        new void Add(TKey key, TValue value);
        new bool ContainsKey(TKey key);
        new bool Remove(TKey key);
        new bool TryGetValue(TKey key, out TValue value);
        new void Add(KeyValuePair<TKey, TValue> item);
        new void Clear();
        new bool Contains(KeyValuePair<TKey, TValue> item);
        new void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);
        new bool Remove(KeyValuePair<TKey, TValue> item);
        new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
    }
}
