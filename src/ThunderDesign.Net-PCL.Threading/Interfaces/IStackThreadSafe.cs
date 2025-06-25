using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace ThunderDesign.Net.Threading.Interfaces
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public interface IStackThreadSafe<T> : IEnumerable<T>, System.Collections.ICollection, IReadOnlyCollection<T>
#elif NETSTANDARD1_3_OR_GREATER
    public interface IStackThreadSafe<T> : IEnumerable<T>, System.Collections.ICollection
#endif

#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER
    {
        #region properties
        new int Count { get; }
        #endregion

        #region methods
        void Clear();
        bool Contains(T item);
        void CopyTo(T[] array, int arrayIndex);
        T Peek();
        T Pop();
        void Push(T item);
        T[] ToArray();
        void TrimExcess();
#if NET6_0_OR_GREATER
        bool TryPeek(out T result);
        bool TryPop(out T result);
#endif
        #endregion
    }
#endif
}