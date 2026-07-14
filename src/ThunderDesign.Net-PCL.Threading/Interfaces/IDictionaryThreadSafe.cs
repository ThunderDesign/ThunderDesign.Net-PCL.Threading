using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
//using static System.Collections.Generic.Dictionary<TKey, TValue>;
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

    public interface IDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionaryThreadSafe where TKey : notnull
    {
        new TValue this[TKey key] { get; set; }
        new int Count { get; }
        new void Add(TKey key, TValue value);
        new bool ContainsKey(TKey key);
        new bool Remove(TKey key);
#if NET6_0_OR_GREATER 
        new bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value);
#else
        new bool TryGetValue(TKey key, out TValue value);
#endif

        new void Clear();
    }
}
