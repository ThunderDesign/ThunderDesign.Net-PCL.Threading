using System.Collections.Generic;
using System.Threading;
using ThunderDesign.Net_PCL.Threading.Interfaces;

namespace ThunderDesign.Net_PCL.Threading.Collections
{
#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER
    public class StackThreadSafe<T> : Stack<T>, IStackThreadSafe<T>
    {
        #region constructors
        public StackThreadSafe() : base() { }

        public StackThreadSafe(IEnumerable<T> collection) : base(collection) { }

        public StackThreadSafe(int capacity) : base(capacity) { }
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

        public new T Pop()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Pop();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void Push(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Push(item);
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

#if NET6_0_OR_GREATER
        public new bool TryPeek(out T result)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.TryPeek(out result);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool TryPop(out T result)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.TryPop(out result);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }
#endif
        #endregion

        #region variables
        protected readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #endregion
    }
#endif
}