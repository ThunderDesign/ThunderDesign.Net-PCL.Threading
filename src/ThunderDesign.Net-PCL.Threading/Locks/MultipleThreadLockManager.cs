using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Objects;

namespace ThunderDesign.Net.Threading.Locks
{
    public class MultipleThreadLockManager<TKey, TValue> : DisposableThreadObject, IMultipleThreadLockManager<TKey, TValue> where TKey : notnull where TValue : ISingleThreadLock, new()
    {
        protected readonly DictionaryThreadSafe<TKey, TValue> ThreadLocks = new DictionaryThreadSafe<TKey, TValue>();

        public virtual async Task LockAsync(TKey key, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var threadLock = ThreadLocks.GetOrAdd(key, _ => new TValue());

            using var lockCts = LinkTokenSourceDisposing(cancellationToken);

            try
            {
                await threadLock.LockAsync(lockCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                ThrowIfDisposed();
                throw;
            }
        }

        public virtual void Unlock(TKey key)
        {
            ThrowIfDisposed();

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (ThreadLocks.TryGetValue(key, out var threadLock))
            {
                if (threadLock?.IsLocked ?? false)
                    threadLock.Unlock();
                else
                    throw new SynchronizationLockException($"The lock for the specified key: {key} is not currently locked.");
            }
            else
            {
                throw new KeyNotFoundException($"No lock found for the specified key: {key}");
            }
        }

        protected override void Dispose(bool disposing, bool setDisposed = true)
        {
            base.Dispose(disposing, setDisposed: false);
            if (!Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                    foreach (var threadLock in ThreadLocks.Values)
                    {
                        threadLock?.Dispose();
                    }

                    ThreadLocks.Clear();
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
