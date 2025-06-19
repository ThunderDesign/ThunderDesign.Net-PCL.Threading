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
        public ObservableCollectionThreadSafe(bool waitOnNotifying = true) : base()
        {
            _waitOnNotifyingRef = waitOnNotifying;
        }

        public ObservableCollectionThreadSafe(List<T> list, bool waitOnNotifying = true)
            : base((list != null) ? new List<T>(list.Count) : list)
        {
            _waitOnNotifyingRef = waitOnNotifying;
            // doesn't copy the list (contrary to the documentation) - it uses the
            // list directly as its storage.  So we do the copying here.
            // 
            CopyFrom(list);
        }

        public ObservableCollectionThreadSafe(IEnumerable<T> collection, bool waitOnNotifying = true)
        {
            _waitOnNotifyingRef = waitOnNotifying;
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
        public bool WaitOnNotifying
        {
            get { return this.GetProperty(ref _waitOnNotifyingRef, _Locker); }
            set { this.SetProperty(ref _waitOnNotifyingRef, value, _Locker, true); }
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
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
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
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public new void Add(T item)
        {
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                base.Add(item);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, IndexOf(item)));
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
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public new void Insert(int index, T item)
        {
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                base.Insert(index, item);
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public new bool Remove(T item)
        {
            var result = false;
            int index = -1;
            T removedItem = default;
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
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
                    OnPropertyChanged(_IndexerName);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
                }
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
            return result;
        }

        public new void RemoveAt(int index)
        {
            T removedItem = default;
            var notifyAndWait = WaitOnNotifying;

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
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
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            this.NotifyPropertyChanged(PropertyChanged, propertyName, WaitOnNotifying);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.NotifyCollectionChanged(CollectionChanged, args, WaitOnNotifying);
        }

#if NET6_0_OR_GREATER
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var notifyAndWait = WaitOnNotifying;
            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    foreach (var item in collection)
                    {
                        base.Add(item);
                    }
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(collection)));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public void RemoveRange(int index, int count)
        {
            var notifyAndWait = WaitOnNotifying;
            List<T> removedItems = new List<T>();

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        removedItems.Add(this[index]);
                        base.RemoveAt(index);
                    }
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                OnPropertyChanged(nameof(Count));
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        public void MoveRange(int oldIndex, int count, int newIndex)
        {
            var notifyAndWait = WaitOnNotifying;
            List<T> movedItems = new List<T>();

            if (notifyAndWait)
                _ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                _ReaderWriterLockSlim.EnterWriteLock();
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        movedItems.Add(this[oldIndex]);
                        base.RemoveAt(oldIndex);
                    }
                    for (int i = 0; i < movedItems.Count; i++)
                    {
                        base.Insert(newIndex + i, movedItems[i]);
                    }
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
                }
                OnPropertyChanged(_IndexerName);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItems, newIndex, oldIndex));
            }
            finally
            {
                if (notifyAndWait)
                    _ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }
#endif
        #endregion

        #region variables
        protected readonly object _Locker = new object();
        protected bool _waitOnNotifyingRef = true;
        // This must agree with Binding.IndexerName.  It is declared separately
        // here so as to avoid a dependency on PresentationFramework.dll.
        private const string _IndexerName = "Item[]";
        #endregion
    }
}
