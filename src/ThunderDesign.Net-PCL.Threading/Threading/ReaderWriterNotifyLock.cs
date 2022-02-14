using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using ThunderDesign.Net.Threading.Extentions;

namespace ThunderDesign.Net.Threading.Threading
{
    public class ReaderWriterNotifyLock_CollectionChangedInfo
    {
        public INotifyCollectionChanged Sender { get; set; }
        public NotifyCollectionChangedEventHandler Handler { get; set; }
        public NotifyCollectionChangedEventArgs Args { get; set; }
    }

    public class ReaderWriterNotifyLock
    {
        #region properties
        // Allow ReadLock if your able to get a WriteLock
        protected bool CanGetReadLock { get { return CanGetWriteLock; } }
        // Allow UpgradeLock if you already have one or nobody else has one. Also allow it if you already have a WriteLock
        protected bool CanGetUpgradeLock { get { return (_UpgradeLockOwnerId_Interlocked == -1 || HaveUpgradeLock) || HaveWriteLock; } }// Environment.CurrentManagedThreadId); } }
        // Allow WriteLock if you have an Upgrade Lock. Also allow if you already have a WriteLock or nobody else has one.
        protected bool CanGetWriteLock { get { return HaveUpgradeLock || (_WriteLockOwnerId_Interlocked == -1 || HaveWriteLock); } }// Environment.CurrentManagedThreadId); } }
        protected bool HaveUpgradeLock { get { return _UpgradeLockOwnerId_Interlocked == Thread.CurrentThread.ManagedThreadId; } }
        protected bool HaveWriteLock { get { return _WriteLockOwnerId_Interlocked == Thread.CurrentThread.ManagedThreadId; } }
        protected bool HasCollectionChanges { get { return (this.GetProperty(ref _CollectionChangedList, _CollectionChangeLocker).Count > 0); } }
        #endregion

        #region methods
        public void EnterReadLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_ReadLocker, ref lockWasTaken);
                WaitForRead();
                Interlocked.Increment(ref _TotalReadCount_Interlocked);
                _ReadCount_ThreadLocal.Value++;
            }
            finally
            {
                if (lockWasTaken)
                    Monitor.Exit(_ReadLocker);
            }
        }

        public void EnterUpgradeableReadLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_UpgradeableReadLocker, ref lockWasTaken);
                WaitForUpgradeableRead();
                Interlocked.Exchange(ref _UpgradeLockOwnerId_Interlocked, Thread.CurrentThread.ManagedThreadId);
                Interlocked.Increment(ref _UpgradeCount_Interlocked);
            }
            finally
            {
                if (lockWasTaken)
                    Monitor.Exit(_UpgradeableReadLocker);
            }
        }

        public void EnterWriteLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_WriteLocker, ref lockWasTaken);
                WaitForWrite();
                Interlocked.Exchange(ref _WriteLockOwnerId_Interlocked, Thread.CurrentThread.ManagedThreadId);
                Interlocked.Increment(ref _WriteCount_Interlocked);
            }
            finally
            {
                if (lockWasTaken)
                    Monitor.Exit(_WriteLocker);
            }
        }
        #endregion

        public void ExitReadLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_ReadLocker, ref lockWasTaken);
                if (_ReadCount_ThreadLocal.Value <= 0)
                    throw new SynchronizationLockException("ExitReadLock called too many times.");
                _ReadCount_ThreadLocal.Value--;
                Interlocked.Decrement(ref _TotalReadCount_Interlocked);
            }
            finally
            {
                if (lockWasTaken)
                    Monitor.Exit(_ReadLocker);
            }
        }

        public void ExitUpgradeableReadLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_UpgradeableReadLocker, ref lockWasTaken);
                if (!HaveUpgradeLock)
                    throw new SynchronizationLockException("ExitUpgradeableReadLock failed because another thread has it locked.");
                if (_UpgradeCount_Interlocked <= 0)
                    throw new SynchronizationLockException("ExitUpgradeableReadLock called too many times.");

                Interlocked.Decrement(ref _UpgradeCount_Interlocked);
                if (_UpgradeCount_Interlocked == 0)
                    Interlocked.Exchange(ref _UpgradeLockOwnerId_Interlocked, -1);
            }
            finally
            {
                WakeUpAppropriateWaiters();
                if (lockWasTaken)
                    Monitor.Exit(_UpgradeableReadLocker);
            }
        }

        public void ExitWriteLock()
        {
            bool lockWasTaken = false;
            try
            {
                Monitor.Enter(_WriteLocker, ref lockWasTaken);
                if (!HaveWriteLock)
                    throw new SynchronizationLockException("ExitWriteLock failed because another thread has it locked.");
                if (_WriteCount_Interlocked <= 0)
                    throw new SynchronizationLockException("ExitWriteLock called too many times.");

                Interlocked.Decrement(ref _WriteCount_Interlocked);
                if (_WriteCount_Interlocked == 0)
                    Interlocked.Exchange(ref _WriteLockOwnerId_Interlocked, -1);
            }
            finally
            {
                WakeUpAppropriateWaiters();
                if (lockWasTaken)
                    Monitor.Exit(_WriteLocker);
            }
        }

        public void AddCollectionChanged(
            INotifyCollectionChanged sender,
            NotifyCollectionChangedEventHandler handler,
            NotifyCollectionChangedEventArgs args)
        {
            lock (_CollectionChangeLocker)
            {
                if (sender != null && handler != null & args != null)
                    _CollectionChangedList.Add(new ReaderWriterNotifyLock_CollectionChangedInfo() { Sender = sender, Handler = handler, Args = args });
            }
        }

        protected virtual void WaitForRead()
        {
            if (!CanGetReadLock)
            {
                try
                {
                    Interlocked.Increment(ref _WaitingReadCount_Interlocked);
                    do
                    {
                        Monitor.Wait(_ReadLocker);

                    } while (!CanGetReadLock);
                }
                finally
                {
                    Interlocked.Decrement(ref _WaitingReadCount_Interlocked);
                }
            }
        }

        protected virtual void WaitForUpgradeableRead()
        {
            if (!CanGetUpgradeLock)
            {
                try
                {
                    Interlocked.Increment(ref _WaitingUpgradeCount_Interlocked);
                    do
                    {
                        Monitor.Wait(_UpgradeableReadLocker);

                    } while (!CanGetUpgradeLock);
                }
                finally
                {
                    Interlocked.Decrement(ref _WaitingUpgradeCount_Interlocked);
                }
            }
        }

        protected virtual void WaitForWrite()
        {
            if ((!CanGetWriteLock) || (HasCollectionChanges && !HaveWriteLock))
            {
                try
                {
                    Interlocked.Increment(ref _WaitingWriteCount_Interlocked);
                    do
                    {
                        Monitor.Wait(_WriteLocker);

                    } while ((!CanGetWriteLock) || (HasCollectionChanges && !HaveWriteLock));
                }
                finally
                {
                    Interlocked.Decrement(ref _WaitingWriteCount_Interlocked);
                }
            }
        }

        protected virtual void WakeUpAppropriateWaiters()
        {
            if (!HaveWriteLock)
            {
                bool checkForWaitingWriteLocks = true;
                if (!HaveUpgradeLock && _WaitingUpgradeCount_Interlocked > 0)
                {
                    Monitor.Pulse(_UpgradeableReadLocker);
                    checkForWaitingWriteLocks = false;
                }

                if (_WaitingReadCount_Interlocked > 0)
                    Monitor.PulseAll(_ReadLocker);

                lock (_CollectionChangeLocker)
                {
                    if (HasCollectionChanges)
                    {
                        try
                        {
                            foreach (ReaderWriterNotifyLock_CollectionChangedInfo collectionChangedInfo in _CollectionChangedList)
                            {
                                collectionChangedInfo.Handler?.Invoke(collectionChangedInfo.Sender, collectionChangedInfo.Args);
                            }
                        }
                        finally
                        {
                            _CollectionChangedList.Clear();
                        }
                    }
                }

                if (checkForWaitingWriteLocks && _WaitingWriteCount_Interlocked > 0)
                {
                    Monitor.Pulse(_WriteLocker);
                }
            }
        }

        #region variables
        protected int _UpgradeLockOwnerId_Interlocked = -1;
        protected int _WriteLockOwnerId_Interlocked = -1;
        protected ThreadLocal<int> _ReadCount_ThreadLocal = new ThreadLocal<int>() { Value = 0 };
        protected int _TotalReadCount_Interlocked = 0;
        protected int _UpgradeCount_Interlocked = 0;
        protected int _WriteCount_Interlocked = 0;
        protected int _WaitingReadCount_Interlocked = 0;
        protected int _WaitingUpgradeCount_Interlocked = 0;
        protected int _WaitingWriteCount_Interlocked = 0;
        protected readonly object _ReadLocker = new object();
        protected readonly object _UpgradeableReadLocker = new object();
        protected readonly object _WriteLocker = new object();
        protected readonly object _CollectionChangeLocker = new object();
        protected List<ReaderWriterNotifyLock_CollectionChangedInfo> _CollectionChangedList = new List<ReaderWriterNotifyLock_CollectionChangedInfo>();
        #endregion
    }
}
