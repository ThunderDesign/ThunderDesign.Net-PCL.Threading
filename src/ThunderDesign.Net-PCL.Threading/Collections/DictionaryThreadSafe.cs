using System;
using System.Collections.Generic;
using System.ComponentModel;

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif
using System.Threading;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class DictionaryThreadSafe<TKey, TValue> : Dictionary<TKey, TValue>, IDictionaryThreadSafe<TKey, TValue>
    {
        #region constructors
        public DictionaryThreadSafe() : base() { }

        public DictionaryThreadSafe(int capacity) : base(capacity) { }

        public DictionaryThreadSafe(IEqualityComparer<TKey> comparer) : base(comparer) { }

        public DictionaryThreadSafe(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public DictionaryThreadSafe(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        public DictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        #endregion

        #region properties
        public bool IsSynchronized
        {
            get { return true; }
        }

        public new IEqualityComparer<TKey> Comparer
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

        public new KeyCollection Keys
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

        public new ValueCollection Values
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

#if NET8_0_OR_GREATER
        [Obsolete("GetObjectData is obsolete in .Net 8", false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
#elif NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
        [System.Security.SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.GetObjectData(info, context);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
        public override void OnDeserialization(object sender)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                base.OnDeserialization(sender);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

#if NET6_0_OR_GREATER
        public new bool TryAdd(TKey key, TValue value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.TryAdd(key, value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool Remove(TKey key, out TValue value)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Remove(key, out value);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new int EnsureCapacity(int capacity)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.EnsureCapacity(capacity);
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

        public new void TrimExcess(int capacity)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.TrimExcess(capacity);
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
}
