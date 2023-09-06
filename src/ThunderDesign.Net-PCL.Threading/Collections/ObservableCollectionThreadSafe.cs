using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class ObservableCollectionThreadSafe<T> : CollectionThreadSafe<T>, IObservableCollectionThreadSafe<T>
    {
        #region constructors
        public ObservableCollectionThreadSafe(bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true) : base() 
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
        }

        public ObservableCollectionThreadSafe(List<T> list, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true)
            : base((list != null) ? new List<T>(list.Count) : list)
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
            // doesn't copy the list (contrary to the documentation) - it uses the
            // list directly as its storage.  So we do the copying here.
            // 
            CopyFrom(list);
        }

        public ObservableCollectionThreadSafe(IEnumerable<T> collection, bool waitOnNotifyPropertyChanged = true, bool waitOnNotifyCollectionChanged = true)
        {
            _WaitOnNotifyPropertyChanged = waitOnNotifyPropertyChanged;
            _WaitOnNotifyCollectionChanged = waitOnNotifyCollectionChanged;
            if (collection == null)
                throw new ArgumentNullException("collection");

            CopyFrom(collection);
        }
        #endregion

        #region event handlers
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
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
        private void CopyFrom(IEnumerable<T> collection)
        {
            IList<T> items = Items;
            if (collection != null && items != null)
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        items.Add(enumerator.Current);
                    }
                }
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            T removedItem = default;
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                removedItem = this[oldIndex];
                base.RemoveItem(oldIndex);
                base.InsertItem(newIndex, removedItem);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex));
        }

        public new void Add(T item)
        {
            base.Add(item);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, IndexOf(item)));
        }

        public new void Clear()
        {
            base.Clear();
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public new bool Remove(T item)
        {
            var result = false;
            int index = -1;
            T removedItem = default;
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                index = Items.IndexOf(item);
                if (index >= 0)
                    removedItem = this[index];
                result = base.Remove(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
            if (result)
            {
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            }
            return result;
        }

        public new void RemoveAt(int index)
        {
            T removedItem = default;

            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                if (index >= 0 || index < Items.Count)
                    removedItem = this[index];

                base.RemoveAt(index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(IndexerName);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
        }

        public virtual void OnPropertyChanged(string propertyName)
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
        // This must agree with Binding.IndexerName.  It is declared separately
        // here so as to avoid a dependency on PresentationFramework.dll.
        private const string IndexerName = "Item[]";
        #endregion
    }
}
