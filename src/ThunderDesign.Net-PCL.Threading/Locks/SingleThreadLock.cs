using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Objects;

namespace ThunderDesign.Net.Threading.Locks
{

    public class SingleThreadLock : DisposableThreadObject, ISingleThreadLock, IDisposable
    {
        // Using Interlocked to ensure thread-safe access to the _lockedGateableThreadLocks field
        private int _pendingWaitCount;

        private readonly SemaphoreSlim _internalLock = new SemaphoreSlim(1, 1);

        public bool IsLocked => _internalLock?.CurrentCount == 0;

        public virtual async Task LockAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            using var linkedCts = LinkTokenSourceDisposing(cancellationToken);

            try
            {
                await InternalLockWaitAsync(linkedCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                ThrowIfDisposed();
                throw;
            }
        }

        public virtual void Unlock()
        {
            ThrowIfDisposed();

            if (!IsLocked)
                throw new SynchronizationLockException("The lock is not currently held.");

            InternalLockRelease();
        }

        protected void InternalLockDispose()
        {
            // Spin until all threads have decremented the counter and exited the block or until the timeout is reached (1 second)
            SpinWait.SpinUntil(() => Volatile.Read(ref _pendingWaitCount) == 0, 1000);
            if (IsLocked)
                _internalLock?.Release();
            _internalLock?.Dispose();
        }

        protected async Task InternalLockWaitAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _pendingWaitCount);
            try
            {
                await _internalLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                Interlocked.Decrement(ref _pendingWaitCount);
            }
        }

        protected void InternalLockRelease()
        {
            _internalLock?.Release();
        }

        protected override void Dispose(bool disposing, bool setDisposed = true)
        {
            base.Dispose(disposing, setDisposed: false);
            if (!Disposed)
            {
                if (disposing)
                {
                    InternalLockDispose();
                }

                // Dispose unmanaged resources here

                if (setDisposed)
                {
                    Disposed = true;
                }
            }
        }
    }
}
