using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface ICollectionThreadSafe : IList
    {
        new bool IsSynchronized { get; }
    }

    public interface ICollectionThreadSafe<T> : IList<T>, IReadOnlyList<T>, ICollectionThreadSafe
    {
        new T this[int index] { get; set; }
        new int Count { get; }
        new void Add(T item);
        new void Clear();
        new bool Contains(T item);
        new void CopyTo(T[] array, int arrayIndex);
        new IEnumerator<T> GetEnumerator();
        new int IndexOf(T item);
        new void Insert(int index, T item);
        new bool Remove(T item);
        new void RemoveAt(int index);
    }
}
