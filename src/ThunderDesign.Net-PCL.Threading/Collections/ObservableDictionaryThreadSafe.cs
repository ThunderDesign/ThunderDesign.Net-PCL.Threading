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
        #endregion

        #region methods
        public override void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
        }

        public override void Clear()
        {
            base.Clear();
            OnPropertyChanged(nameof(Keys));
            OnPropertyChanged(nameof(Values));
            OnPropertyChanged(nameof(Count));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public override bool Remove(TKey key)
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.NotifyCollectionChanged(CollectionChanged, args);
        }
        #endregion
    }
}
