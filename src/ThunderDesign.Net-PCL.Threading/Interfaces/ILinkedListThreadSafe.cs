using System.Collections;
using System.Collections.Generic;
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace ThunderDesign.Net.Threading.Interfaces
{
#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
    public interface ILinkedListThreadSafe<T> : ICollection<T>, ICollection, IReadOnlyCollection<T>, ISerializable, IDeserializationCallback
#elif NETSTANDARD1_3_OR_GREATER
    public interface ILinkedListThreadSafe<T> : IEnumerable<T>, ICollection<T>
#endif

#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER

    {
        #region properties
        new int Count { get; }
        LinkedListNode<T> First { get; }
        LinkedListNode<T> Last { get; }
        #endregion

        #region methods
        void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode);
        void AddAfter(LinkedListNode<T> node, T value);
        void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode);
        void AddBefore(LinkedListNode<T> node, T value);
        void AddFirst(LinkedListNode<T> node);
        void AddFirst(T value);
        void AddLast(LinkedListNode<T> node);
        void AddLast(T value);
        new void Clear();
        new bool Contains(T value);
        new void CopyTo(T[] array, int index);
        LinkedListNode<T> Find(T value);
        LinkedListNode<T> FindLast(T value);
        void Remove(LinkedListNode<T> node);
        new bool Remove(T value);
        void RemoveFirst();
        void RemoveLast();
        #endregion
    }
#endif
}