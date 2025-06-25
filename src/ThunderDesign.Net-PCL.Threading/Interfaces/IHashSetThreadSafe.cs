using System;
using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace ThunderDesign.Net.Threading.Interfaces
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public interface IHashSetThreadSafe<T> : ICollection<T>, ISet<T>, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
#elif NETSTANDARD1_3_OR_GREATER
    public interface IHashSetThreadSafe<T> : ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>, ISet<T>
#endif

#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER

    {
        #region properties
        new int Count { get; }
        IEqualityComparer<T> Comparer { get; }
        #endregion

        #region methods
        new bool Add(T item);
        new void Clear();
        new bool Contains(T item);
        void CopyTo(T[] array);
        new void CopyTo(T[] array, int arrayIndex);
        void CopyTo(T[] array, int arrayIndex, int count);
        new void ExceptWith(IEnumerable<T> other);
        new void IntersectWith(IEnumerable<T> other);
        new bool IsProperSubsetOf(IEnumerable<T> other);
        new bool IsProperSupersetOf(IEnumerable<T> other);
        new bool IsSubsetOf(IEnumerable<T> other);
        new bool IsSupersetOf(IEnumerable<T> other);
        new bool Overlaps(IEnumerable<T> other);
        new bool Remove(T item);
        int RemoveWhere(Predicate<T> match);
        new bool SetEquals(IEnumerable<T> other);
        new void SymmetricExceptWith(IEnumerable<T> other);
        new void UnionWith(IEnumerable<T> other);
        #endregion
    }
#endif
}