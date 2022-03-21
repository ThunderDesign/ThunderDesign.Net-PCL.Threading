using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using System.Runtime.Serialization;
#endif
namespace ThunderDesign.Net.Threading.Interfaces
{
#if NETSTANDARD2_0 || NETSTANDARD2_1
    public interface IDictionaryThreadSafe : IDictionary, IDeserializationCallback, ISerializable
#else
    public interface IDictionaryThreadSafe : IDictionary
#endif
    {
    }

    public interface IDictionaryThreadSafe<TKey, TValue> : IDictionary<TKey, TValue>, IDictionaryThreadSafe
    {
    }
}
