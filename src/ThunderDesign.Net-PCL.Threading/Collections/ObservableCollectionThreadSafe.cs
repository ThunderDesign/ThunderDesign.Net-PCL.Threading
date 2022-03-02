using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Collections
{
    public class ObservableCollectionThreadSafe<T> : ObservableCollection<T>, IObservableCollectionThreadSafe<T>
    {
        #region constructors
        public ObservableCollectionThreadSafe() : base() { }
        public ObservableCollectionThreadSafe(IEnumerable<T> collection) : base(collection) { }
        public ObservableCollectionThreadSafe(List<T> list) : base(list) { }
        #endregion

        #region event handlers
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public bool IsSynchronized
        {
            get { return true; }
        }

        public new T this[int index]
        {
            get
            {
                _ReaderWriterLockSlim.EnterReadLock();
                try
                {
                    return base[index];
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
                    base[index] = value;
                }
                finally
                {
                    _ReaderWriterLockSlim.ExitWriteLock();
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
        #endregion

        #region methods
        public new void Move(int oldIndex, int newIndex)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Move(oldIndex, newIndex);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new virtual void Add(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Add(item);
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

        public new bool Contains(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.Contains(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void CopyTo(T[] array, int index)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.CopyTo(array, index);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new IEnumerator<T> GetEnumerator()
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

        public new int IndexOf(T item)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return base.IndexOf(item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        public new void Insert(int index, T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                base.Insert(index, item);
            }
            finally
            {
                _ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        public new bool Remove(T item)
        {
            _ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                return base.Remove(item);
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

        public T GetItemByIndex(int index)
        {
            _ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return this[index];
            }
            finally
            {
                _ReaderWriterLockSlim.ExitReadLock();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(PropertyChanged, e.PropertyName);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            this.NotifyCollectionChanged(CollectionChanged, e);
        }
        #endregion

        #region variables
        protected static readonly ReaderWriterLockSlim _ReaderWriterLockSlim = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        #endregion
    }
}
