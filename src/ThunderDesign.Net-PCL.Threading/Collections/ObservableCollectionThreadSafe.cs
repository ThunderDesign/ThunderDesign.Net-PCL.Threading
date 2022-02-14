using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Threading;

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
        protected override event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region properties
        public new T this[int index]
        {
            get
            {
                _ReaderWriterNotifyLock.EnterReadLock();
                try
                {
                    return base[index];
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
                    base[index] = value;
                }
                finally
                {
                    _ReaderWriterNotifyLock.ExitWriteLock();
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
        #endregion

        #region methods
        public new void Move(int oldIndex, int newIndex)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.Move(oldIndex, newIndex);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
        }

        public new virtual void Add(T item)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.Add(item);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
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
        }

        public new bool Contains(T item)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.Contains(item);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public new void CopyTo(T[] array, int index)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.CopyTo(array, index);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
        }

        public new IEnumerator<T> GetEnumerator()
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

        public new int IndexOf(T item)
        {
            _ReaderWriterNotifyLock.EnterReadLock();
            try
            {
                return base.IndexOf(item);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitReadLock();
            }
        }

        public new void Insert(int index, T item)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.Insert(index, item);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
        }

        public new bool Remove(T item)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                return base.Remove(item);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
        }

        public new void RemoveAt(int index)
        {
            _ReaderWriterNotifyLock.EnterWriteLock();
            try
            {
                base.RemoveAt(index);
            }
            finally
            {
                _ReaderWriterNotifyLock.ExitWriteLock();
            }
        }

        public T GetItemByIndex(int index)
        {
            return this[index];
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(PropertyChanged, e.PropertyName);
        }
        #endregion

        #region variables
        protected static readonly ReaderWriterNotifyLock _ReaderWriterNotifyLock = new ReaderWriterNotifyLock();
        #endregion
    }
}
