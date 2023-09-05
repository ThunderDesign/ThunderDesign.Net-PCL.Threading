using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class CollectionThreadSafe<T> : Collection<T>, ICollectionThreadSafe<T>
    {
        #region constructors
        public CollectionThreadSafe() : base() { }

        public CollectionThreadSafe(IList<T> list) : base(list) { }
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

        public new void CopyTo(T[] array, int index)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.CopyTo(array, index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
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

        public T GetItemByIndex(int index)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return this[index];
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
}
