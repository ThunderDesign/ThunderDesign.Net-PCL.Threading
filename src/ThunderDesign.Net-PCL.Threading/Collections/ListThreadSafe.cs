using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ThunderDesign.Net_PCL.Threading.Interfaces;

namespace ThunderDesign.Net_PCL.Threading.Collections
{
    public class ListThreadSafe<T> : List<T>, IListThreadSafe<T>
    {
        #region constructors
        public ListThreadSafe() : base() { }

        public ListThreadSafe(IEnumerable<T> collection) : base(collection) { }

        public ListThreadSafe(int capacity) : base(capacity) { }
        #endregion

        #region properties
        public bool IsSynchronized
        {
            get { return true; }
        }

        public new T this[int index]
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base[index];
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
            set
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    base[index] = value;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
            }
        }

        public new int Count
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Count;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }

        public new int Capacity
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Capacity;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
            set
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    base.Capacity = value;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
            }
        }
        #endregion

        #region methods
        public new void Add(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Add(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddRange(collection);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

#if NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD2_0 || NETSTANDARD2_1
        public new ReadOnlyCollection<T> AsReadOnly()
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.AsReadOnly();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

        public new int BinarySearch(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.BinarySearch(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int BinarySearch(T item, IComparer<T> comparer)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.BinarySearch(item, comparer);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.BinarySearch(index, count, item, comparer);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void Clear()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Clear();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool Contains(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Contains(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

#if NETSTANDARD2_0 || NETSTANDARD2_1
        public new List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.ConvertAll(converter);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

        public new void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.CopyTo(index, array, arrayIndex, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void CopyTo(T[] array, int arrayIndex)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.CopyTo(array, arrayIndex);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void CopyTo(T[] array)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.CopyTo(array);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool Exists(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Exists(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new T Find(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Find(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new List<T> FindAll(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindAll(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindIndex(startIndex, count, match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindIndex(int startIndex, Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindIndex(startIndex, match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindIndex(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindIndex(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new T FindLast(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindLast(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindLastIndex(startIndex, count, match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindLastIndex(int startIndex, Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindLastIndex(startIndex, match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int FindLastIndex(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindLastIndex(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

#if NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD2_0 || NETSTANDARD2_1
        public new void ForEach(Action<T> action)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.ForEach(action);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

        public new Enumerator GetEnumerator()
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.GetEnumerator();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new List<T> GetRange(int index, int count)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.GetRange(index, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int IndexOf(T item, int index, int count)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOf(item, index, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int IndexOf(T item, int index)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOf(item, index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int IndexOf(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOf(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void Insert(int index, T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Insert(index, item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.InsertRange(index, collection);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new int LastIndexOf(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.LastIndexOf(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int LastIndexOf(T item, int index)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.LastIndexOf(item, index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int LastIndexOf(T item, int index, int count)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.LastIndexOf(item, index, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool Remove(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Remove(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new int RemoveAll(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.RemoveAll(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void RemoveAt(int index)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.RemoveAt(index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void RemoveRange(int index, int count)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.RemoveRange(index, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Reverse(int index, int count)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Reverse(index, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Reverse()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Reverse();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Sort(Comparison<T> comparison)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Sort(comparison);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Sort(int index, int count, IComparer<T> comparer)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Sort(index, count, comparer);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Sort()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Sort();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Sort(IComparer<T> comparer)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Sort(comparer);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new T[] ToArray()
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.ToArray();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void TrimExcess()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.TrimExcess();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool TrueForAll(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.TrueForAll(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }
#endregion

#region variables
        protected static readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
#endregion
    }
}
