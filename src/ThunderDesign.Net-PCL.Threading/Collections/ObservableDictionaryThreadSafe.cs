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
        public ObservableDictionaryThreadSafe(bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base() 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableDictionaryThreadSafe(int capacity, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(capacity) 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableDictionaryThreadSafe(IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(comparer) 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(dictionary) 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableDictionaryThreadSafe(int capacity, IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(capacity, comparer) 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableDictionaryThreadSafe(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base(dictionary, comparer) 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
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
        }

        public bool WaitOnNotifyPropertyChanged
        {
            get { return this.GetProperty(ref _WaitOnNotifyPropertyChanged, _Locker); }
            set { this.SetProperty(ref _WaitOnNotifyPropertyChanged, value, _Locker, true); }
        }

        public bool WaitOnNotifyCollectionChanged
        {
            get { return this.GetProperty(ref _WaitOnNotifyCollectionChanged, _Locker); }
            set { this.SetProperty(ref _WaitOnNotifyCollectionChanged, value, _Locker, true); }
        }
        #endregion

        #region methods
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        public new void Clear()
        {
            base.Clear();
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new bool Remove(TKey key)
        {
            bool result = false;
            TValue value;
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
            return result;
        }

        public void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitOnNotifyPropertyChanged);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.NotifyCollectionChanged(CollectionChanged, args, WaitOnNotifyCollectionChanged);
        }
        #endregion

        #region variables
        protected readonly object _Locker = new object();
        protected bool _WaitOnNotifyPropertyChanged = true;
        protected bool _WaitOnNotifyCollectionChanged = true;
        #endregion
    }
}
