using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Threading;

namespace ThunderDesign.Net.Threading.Collections
{
    public class ObservableDictionaryThreadSafe<TKey, TValue> : Dictionary<TKey, TValue>, IObservableDictionaryThreadSafe<TKey, TValue>
    {
        #region constructors
        public ObservableDictionaryThreadSafe() : base() { }
        public ObservableDictionaryThreadSafe(int capacity) : base(capacity) { }
        public ObservableDictionaryThreadSafe(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public ObservableDictionaryThreadSafe(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }
        #endregion

        #region event handlers
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public new IEqualityComparer<TKey> Comparer
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base.Comparer;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitReadLock();
                }
            }
        }

        public new int Count
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base.Count;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitReadLock();
                }
            }
        }

        public new KeyCollection Keys
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base.Keys;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitReadLock();
                }
            }
        }

        public new ValueCollection Values
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base.Values;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitReadLock();
                }
            }
        }

        public new TValue this[TKey key]
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base[key];
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitReadLock();
                }
            }
            set
            {
                _ReaderWriterNotifyLock.EnterWriteLock();
                try
                {
                    base[key] = value;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitWriteLock();
                }
                OnPropertyChanged(nameof(Values));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, key));
            }
        }
        #endregion

        #region methods
        public new void Add(TKey key, TValue value)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.Add(key, value);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, key));
        }

        public new void Clear()
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.Clear();
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new bool ContainsKey(TKey key)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.ContainsKey(key);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public new bool ContainsValue(TValue value)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.ContainsValue(value);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public new Enumerator GetEnumerator()
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.GetEnumerator();
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        [System.Security.SecurityCritical]  // auto-generated_required
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                base.GetObjectData(info, context);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public override void OnDeserialization(Object sender)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                base.OnDeserialization(sender);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public new bool Remove(TKey key)
        {
            bool result = false;
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                result = base.Remove(key);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, key));
            return result;
        }

        public new bool TryGetValue(TKey key, out TValue value)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.TryGetValue(key, out value);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.NotifyCollectionChanged(CollectionChanged, args);
        }

        #endregion

        #region variables
        protected static readonly ReaderWriterNotifyLock _ReaderWriterNotifyLock = new ReaderWriterNotifyLock();

        #endregion
    }
}
