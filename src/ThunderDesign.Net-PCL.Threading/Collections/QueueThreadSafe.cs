using System.Collections.Generic;
using System.Threading;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class QueueThreadSafe<T> : Queue<T>, IQueueThreadSafe<T>
    {
        #region constructors
        public QueueThreadSafe() : base() { }

        public QueueThreadSafe(IEnumerable<T> collection) : base(collection) { }

        public QueueThreadSafe(int capacity) : base(capacity) { }
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
        #endregion

        #region methods
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

        public new T Dequeue()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Dequeue();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Enqueue(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Enqueue(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

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

        public new T Peek()
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Peek();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
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
        #endregion

        #region variables
        protected readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #endregion
    }
}
