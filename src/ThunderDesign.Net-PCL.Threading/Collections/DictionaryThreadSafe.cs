using System;
using System.Collections;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

#endif

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
using System.Runtime.Serialization;
#endif
using System.Threading;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class DictionaryThreadSafe<TKey, TValue> : Dictionary<TKey, TValue>, IDictionaryThreadSafe<TKey, TValue> where TKey : notnull
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

        bool ICollection.IsSynchronized
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

#if NET9_0_OR_GREATER
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
        }
#endif

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

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER

#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

#if NET9_0_OR_GREATER
        public new AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>()
            where TAlternateKey : notnull, allows ref struct
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.GetAlternateLookup<TAlternateKey>();
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
        public new bool TryGetAlternateLookup<TAlternateKey>(
            out AlternateLookup<TAlternateKey> lookup)
            where TAlternateKey : notnull, allows ref struct
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.TryGetAlternateLookup<TAlternateKey>(out lookup);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }
#endif

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                if (!base.TryGetValue(key, out TValue? value))
                {
                    value = valueFactory(key);
                    Add(key, value);
                }
                return value;
            }
            finally
            {
                _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

#if NETSTANDARD2_0_OR_GREATER || NET6_0_OR_GREATER
        public override void OnDeserialization(object? sender)
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

#if NET6_0_OR_GREATER
        public new bool Remove(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
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
#endif

#if NET6_0_OR_GREATER 
        public new bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
#else
        public new bool TryGetValue(TKey key, out TValue value)
#endif
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

        public new void TrimExcess() => TrimExcess(Count);

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
