using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IDictionaryThreadSafe : IDictionary, IDeserializationCallback, ISerializable
    {
    }

    public interface IDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDictionaryThreadSafe
    {
    }
}
