using System.Collections.Generic;
using System.Threading;

namespace ThunderDesign.Net_PCL.Threading.Collections
{
    public class SortedListThreadSafe<TKey, TValue> : SortedList<TKey, TValue>
    {
        #region constructors
        public SortedListThreadSafe() : base() { }

        public SortedListThreadSafe(IComparer<TKey> comparer) : base(comparer) { }

        public SortedListThreadSafe(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public SortedListThreadSafe(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer) : base(dictionary, comparer) { }

        public SortedListThreadSafe(int capacity) : base(capacity) { }

        public SortedListThreadSafe(int capacity, IComparer<TKey> comparer) : base(comparer) { }
        #endregion

        #region properties
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

        public new IComparer<TKey> Comparer
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

        public new TValue this[TKey key]
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base[key];
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
                    base[key] = value;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
            }
        }

        public new IList<TKey> Keys
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Keys;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }

        public new IList<TValue> Values
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base.Values;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitReadLock();
                }
            }
        }
        #endregion

        #region methods
        public new void Add(TKey key, TValue value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Add(key, value);
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

        public new bool ContainsKey(TKey key)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.ContainsKey(key);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool ContainsValue(TValue value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.ContainsValue(value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
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

        public new int IndexOfKey(TKey key)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOfKey(key);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new int IndexOfValue(TValue value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOfValue(value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new bool Remove(TKey key)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Remove(key);
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

        public new bool TryGetValue(TKey key, out TValue value)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.TryGetValue(key, out value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
        #endregion

        #region variables
        protected static readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #endregion
    }
}
