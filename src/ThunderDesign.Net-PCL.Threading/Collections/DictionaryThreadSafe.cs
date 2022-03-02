using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        public new virtual void Add(TKey key, TValue value)
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

        public new virtual void Clear()
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

        [System.Security.SecurityCritical]  // auto-generated_required
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

        public override void OnDeserialization(Object sender)
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

        public new virtual bool Remove(TKey key)
        {
            bool result = false;
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                result = base.Remove(key);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
            return result;
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
