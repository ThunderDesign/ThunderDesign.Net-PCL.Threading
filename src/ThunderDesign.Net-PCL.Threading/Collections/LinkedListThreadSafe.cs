using System.Collections.Generic;
using System.Threading;
using ThunderDesign.Net_PCL.Threading.Interfaces;

namespace ThunderDesign.Net_PCL.Threading.Collections
{
#if NETSTANDARD1_3_OR_GREATER || NET6_0_OR_GREATER
    public class LinkedListThreadSafe<T> : LinkedList<T>, ILinkedListThreadSafe<T>
    {
        #region constructors
        public LinkedListThreadSafe() : base() { }

        public LinkedListThreadSafe(IEnumerable<T> collection) : base(collection) { }
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

        public new LinkedListNode<T> First
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.First;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }

        public new LinkedListNode<T> Last
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Last;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }
        #endregion

        #region methods
        public new void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddAfter(node, newNode);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddAfter(LinkedListNode<T> node, T value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddAfter(node, value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddBefore(node, newNode);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddBefore(LinkedListNode<T> node, T value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddBefore(node, value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddFirst(LinkedListNode<T> node)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddFirst(node);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddFirst(T value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddFirst(value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddLast(LinkedListNode<T> node)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddLast(node);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void AddLast(T value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.AddLast(value);
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

        public new bool Contains(T value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Contains(value);
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

        public new LinkedListNode<T> Find(T value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Find(value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new LinkedListNode<T> FindLast(T value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.FindLast(value);
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

        public new void Remove(LinkedListNode<T> node)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Remove(node);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool Remove(T value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Remove(value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void RemoveFirst()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.RemoveFirst();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new void RemoveLast()
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.RemoveLast();
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
#endif
}