using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ThunderDesign.Net.Threading.Extentions;

namespace ThunderDesign.Net.Threading.Objects
{
    public abstract class DisposableThreadObject : ThreadObject, IDisposable
    {
        private CancellationTokenSource? _disposingCtsCached = null;

        protected readonly string ClassNameCashed;
        private bool _disposed = false;

        protected DisposableThreadObject()
        {
            ClassNameCashed = this.GetType().Name;
        }

        ~DisposableThreadObject()
        {
            Dispose(disposing: false);
        }

        protected bool Disposed
        {
            get { return this.GetProperty(ref _disposed, _Locker); }
            set { this.SetProperty(ref _disposed, value, _Locker); }
        }

        protected CancellationTokenSource DisposingCts => DisposingCtsCache ??= new CancellationTokenSource();

        protected CancellationTokenSource? DisposingCtsCache
        {
            get { return this.GetProperty(ref _disposingCtsCached, _Locker); }
            private set { this.SetProperty(ref _disposingCtsCached, value, _Locker); }
        }

        protected CancellationTokenSource LinkTokenSourceDisposing(CancellationToken cancellationToken)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(DisposingCts.Token, cancellationToken);
        }

        protected void ThrowIfDisposed()
        {
            if (Disposed || DisposingCtsCache?.IsCancellationRequested == true)
                throw new ObjectDisposedException(ClassNameCashed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing, bool setDisposed = true)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here
                    DisposingCtsCache?.Cancel();
                    DisposingCtsCache?.Dispose();
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
