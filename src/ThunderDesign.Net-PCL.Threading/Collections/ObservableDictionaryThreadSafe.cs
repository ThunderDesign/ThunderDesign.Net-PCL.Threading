using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class ObservableDictionaryThreadSafe<TKey, TValue> : DictionaryThreadSafe<TKey, TValue>, IObservableDictionaryThreadSafe<TKey, TValue>
    {
        #region constructors
        public ObservableDictionaryThreadSafe(bool waitOnNotifying = true) : base()
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableDictionaryThreadSafe(int capacity, bool waitOnNotifying = true) : base(capacity)
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableDictionaryThreadSafe(IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(comparer)
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, bool waitOnNotifying = true) : base(dictionary)
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableDictionaryThreadSafe(int capacity, IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(capacity, comparer)
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, bool waitOnNotifying = true) : base(dictionary, comparer)
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }
        #endregion

        #region event handlers
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties
        public new TValue this[TKey key]
        {
            get
            {
                return base[key];
            }
            set
            {
                TValue originalValue;
                var notifyAndWait = WaitOnNotifying;

                if (notifyAndWait)
                    _ReaderWriterLockSlim.EnterUpgradeableReadLock();
                try
                {
                    _ReaderWriterLockSlim.EnterWriteLock();
                    try
                    {
                        originalValue = base[key];
                        base[key] = value;
                    }
                    finally
                    {
                        _ReaderWriterLockSlim.ExitWriteLock();
                    }
                    OnPropertyChanged(nameof(Values));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalValue));
                }
                finally
                {
                    if (notifyAndWait)
                        _ReaderWriterLockSlim.ExitUpgradeableReadLock();
                }
            }
        }

        public bool WaitOnNotifying
        {
            get { return this.GetProperty(ref _waitOnNotifyingRef, _Locker); }
            set { this.SetProperty(ref _waitOnNotifyingRef, value, _Locker, true); }
        }
        #endregion

        #region methods
        public new void Add(TKey key, TValue value)
        {
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                base.Add(key, value);
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public new void Clear()
        {
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                base.Clear();
                OnPropertyChanged(nameof(Keys));
                OnPropertyChanged(nameof(Values));
                OnPropertyChanged(nameof(Count));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public new bool Remove(TKey key)
        {
            bool result = false;
            TValue value;
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    if (TryGetValue(key, out value))
                        result = base.Remove(key);
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                if (result)
                {
                    OnPropertyChanged(nameof(Keys));
                    OnPropertyChanged(nameof(Values));
                    OnPropertyChanged(nameof(Count));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
                }
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
            return result;
        }

#if NET6_0_OR_GREATER
        public new bool TryAdd(TKey key, TValue value)
        {
            var notifyAndWait = WaitOnNotifying;
            bool result;
            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    result = base.TryAdd(key, value);
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                if (result)
                {
                    OnPropertyChanged(nameof(Keys));
                    OnPropertyChanged(nameof(Values));
                    OnPropertyChanged(nameof(Count));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                }
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
            return result;
        }

        public new bool Remove(TKey key, out TValue value)
        {
            var notifyAndWait = WaitOnNotifying;
            bool result;
            value = default;
            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    result = base.Remove(key, out value);
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                if (result)
                {
                    OnPropertyChanged(nameof(Keys));
                    OnPropertyChanged(nameof(Values));
                    OnPropertyChanged(nameof(Count));
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value));
                }
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
            return result;
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

        public virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitOnNotifying);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.NotifyCollectionChanged(CollectionChanged, args, WaitOnNotifying);
        }
        #endregion

        #region variables
        protected readonly object _Locker = new object();
        protected bool _waitOnNotifyingRef = true;
        #endregion
    }
}
