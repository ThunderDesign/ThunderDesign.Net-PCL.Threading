using System;
using System.Collections.Generic;
using System.Threading;
using ThunderDesign.Net_PCL.Threading.Interfaces;

namespace ThunderDesign.Net_PCL.Threading.Collections
{
#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER
    public class HashSetThreadSafe<T> : HashSet<T>, IHashSetThreadSafe<T>
    {
        #region constructors
        public HashSetThreadSafe() : base() { }

        public HashSetThreadSafe(IEnumerable<T> collection) : base(collection) { }

        public HashSetThreadSafe(IEqualityComparer<T> comparer) : base(comparer) { }

        public HashSetThreadSafe(IEnumerable<T> collection, IEqualityComparer<T> comparer) : base(collection, comparer) { }
        #endregion

        #region properties
        public bool IsSynchronized
        {
            get { return true; }
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

        public new IEqualityComparer<T> Comparer
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Comparer;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }
        #endregion

        #region methods
        public new bool Add(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Add(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
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

        public new void CopyTo(T[] array, int arrayIndex, int count)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.CopyTo(array, arrayIndex, count);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void ExceptWith(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.ExceptWith(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void IntersectWith(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.IntersectWith(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool IsProperSubsetOf(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IsProperSubsetOf(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool IsProperSupersetOf(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IsProperSupersetOf(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool IsSubsetOf(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IsSubsetOf(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool IsSupersetOf(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IsSupersetOf(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool Overlaps(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Overlaps(other);
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

        public new int RemoveWhere(Predicate<T> match)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.RemoveWhere(match);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool SetEquals(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.SetEquals(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void SymmetricExceptWith(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.SymmetricExceptWith(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void UnionWith(IEnumerable<T> other)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.UnionWith(other);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new IEnumerator<T> GetEnumerator()
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
        #endregion

        #region variables
        protected readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #endregion
    }
#endif
}