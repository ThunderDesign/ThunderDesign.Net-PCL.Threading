using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Collections;
using ThunderDesign.Net.Threading.Extentions;
using ThunderDesign.Net.Threading.Interfaces;
using ThunderDesign.Net.Threading.Objects;

namespace ThunderDesign.Net.Threading.Locks
{
    internal interface IInternalGateKeeperLink
    {
        internal Task EnterGateAsync(CancellationToken cancellationToken = default);
        internal void ExitGate();
    }

    public class GateKeeper<TKey, TValue> : DisposableThreadObject, IGateKeeper<TKey, TValue>, IInternalGateKeeperLink, IDisposable where TKey : notnull where TValue : IGatedThreadLock, new()
    {
        private bool _isGateOpen = true;

        // Using Interlocked to ensure thread-safe access to the _lockedGateableThreadLocks field
        private long _lockedGateableThreadLocks = 0;

        private TaskCompletionSource<bool>? _closeGateReadyTcs = null;

        protected readonly DictionaryThreadSafe<TKey, TValue> GateableThreadLocks = new DictionaryThreadSafe<TKey, TValue>();

        protected readonly SemaphoreSlim MasterGateLock = new SemaphoreSlim(1, 1);

        public bool IsGateOpen
        {
            get => this.GetProperty(ref _isGateOpen, _Locker);
            private set => this.SetProperty(ref _isGateOpen, value, _Locker);
        }

        public long LockedGateableThreadLocks => Interlocked.Read(ref _lockedGateableThreadLocks);

        protected TaskCompletionSource<bool>? CloseGateReadyTcs
        {
            get => this.GetProperty(ref _closeGateReadyTcs, _Locker);
            set
            {
                lock (_Locker)
                {
                    var oldValue = _closeGateReadyTcs;
                    if (this.SetProperty(ref _closeGateReadyTcs, value))
                    {
                        oldValue?.TrySetResult(false);
                    }
                }
            }
        }

        public async Task CloseGateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            using var linkedCts = LinkTokenSourceDisposing(cancellationToken);

            var lockAcquired = false;
            try
            {
                await MasterGateLock.WaitAsync(linkedCts.Token).ConfigureAwait(false);
                lockAcquired = true;


                if (LockedGateableThreadLocks > 0)
                {
#if NETSTANDARD1_0
                    CloseGateReadyTcs = new TaskCompletionSource<bool>();
#else
                    CloseGateReadyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
#endif

                    if (LockedGateableThreadLocks == 0)
                    {
                        CloseGateReadyTcs = null;
                        IsGateOpen = false;
                        return;
                    }

                    await Task.WhenAny(CloseGateReadyTcs?.Task ?? Task.FromResult(false), Task.Delay(Timeout.Infinite, linkedCts.Token)).ConfigureAwait(false);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new OperationCanceledException(cancellationToken);
                    }
                    if (!((CloseGateReadyTcs?.Task.IsCompleted == true) && (CloseGateReadyTcs?.Task.Result ?? false == true)))
                    {
                        throw new OperationCanceledException("The gate closing operation was canceled by another operation.");
                    }
                }

                IsGateOpen = false;
            }
            catch (Exception ex)
            {
                if (lockAcquired)
                    MasterGateLock?.Release();

                switch (ex)
                {
                    case OperationCanceledException:
                        ThrowIfDisposed();
                        throw;
                    default:
                        throw;
                }
            }
        }

        public IGatedThreadLock GetOrAdd(TKey key)
        {
            ThrowIfDisposed();

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (GateableThreadLocks.TryGetValue(key, out var existingLock))
            {
                return existingLock;
            }
            else
            {
                var newLock = new TValue();
                if (newLock is IInternalGatedThreadLockLink internalLock)
                {
                    internalLock.LinkGateKeeper(this);
                }
                else
                {
                    throw new InvalidOperationException("The provided gateable thread lock does not inherit from GatedThreadLock Class.");
                }
                GateableThreadLocks.Add(key, newLock);
                return newLock;
            }
        }

        public void OpenGate()
        {
            ThrowIfDisposed();

            if (IsGateOpen)
            {
                throw new InvalidOperationException("The gate is already open.");
            }

            IsGateOpen = true;
            CloseGateReadyTcs = null;
            MasterGateLock?.Release();
        }

        public void Remove(TKey key)
        {
            ThrowIfDisposed();

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (GateableThreadLocks.TryGetValue(key, out var existingLock))
            {
                GateableThreadLocks.Remove(key);
                if (existingLock is IInternalGatedThreadLockLink internalLock)
                {
                    internalLock.UnlinkGateKeeper(this);
                }
                else
                {
                    throw new InvalidOperationException("The provided gateable thread lock does not inherit from GatedThreadLock Class.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"The specified key: {key} does not exist in the gateable thread locks collection.");
            }
        }

        void IInternalGateKeeperLink.ExitGate() => ExitGate();

        private void ExitGate()
        {
            ThrowIfDisposed();

            if (Interlocked.Decrement(ref _lockedGateableThreadLocks) == 0)
            {
                CloseGateReadyTcs?.TrySetResult(true);
            }
        }

        Task IInternalGateKeeperLink.EnterGateAsync(CancellationToken cancellationToken) => EnterGateAsync(cancellationToken);

        private async Task EnterGateAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            using var linkedCts = LinkTokenSourceDisposing(cancellationToken);

            try
            {
                await MasterGateLock.WaitAsync(linkedCts.Token).ConfigureAwait(false);
                try
                {
                    Interlocked.Increment(ref _lockedGateableThreadLocks);
                }
                finally
                {
                    MasterGateLock?.Release();
                }
            }
            catch (OperationCanceledException)
            {
                ThrowIfDisposed();
                throw;
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
                    foreach (var threadLock in GateableThreadLocks.Values)
                    {
                        threadLock?.Dispose();
                    }

                    GateableThreadLocks.Clear();
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
