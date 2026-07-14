using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Locks
{
    internal interface IInternalGatedThreadLockLink
    {
        internal void LinkGateKeeper(IInternalGateKeeperLink gateKeeperLink);
        internal void UnlinkGateKeeper(IInternalGateKeeperLink gateKeeperLink);
    }

    public class GatedThreadLock : SingleThreadLock, IGatedThreadLock, IInternalGatedThreadLockLink, IDisposable
    {
        private IInternalGateKeeperLink? _gateKeeperLink;

        void IInternalGatedThreadLockLink.LinkGateKeeper(IInternalGateKeeperLink gateKeeperLink)
        {
            LinkGateKeeper(gateKeeperLink);
        }

        private void LinkGateKeeper(IInternalGateKeeperLink gateKeeperLink)
        {
            if (gateKeeperLink == null)
                throw new ArgumentNullException(nameof(gateKeeperLink));

            _gateKeeperLink = gateKeeperLink;
        }

        void IInternalGatedThreadLockLink.UnlinkGateKeeper(IInternalGateKeeperLink gateKeeperLink)
        {
            UnlinkGateKeeper(gateKeeperLink);
        }

        private void UnlinkGateKeeper(IInternalGateKeeperLink gateKeeperLink)
        {
            if (gateKeeperLink == null)
                throw new ArgumentNullException(nameof(gateKeeperLink));

            if (_gateKeeperLink != gateKeeperLink)
                throw new InvalidOperationException("The provided lock manager is not registered with this lock.");

            _gateKeeperLink = null;
        }

		public override async Task LockAsync(CancellationToken cancellationToken = default)
		{
			ThrowIfDisposed();

			using var linkedCts = LinkTokenSourceDisposing(cancellationToken);

			var lockAcquired = false;
			try
			{
				await InternalLockWaitAsync(linkedCts.Token).ConfigureAwait(false);
				lockAcquired = true;

				if (_gateKeeperLink == null)
					throw new InvalidOperationException("The gate keeper link is not set.");

				await _gateKeeperLink.EnterGateAsync(linkedCts.Token).ConfigureAwait(false);
			}
			catch (Exception ex)
			{ 
				if (lockAcquired)
					InternalLockRelease();

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

        public override void Unlock()
        {
            base.Unlock();
            _gateKeeperLink?.ExitGate();
        }
    }
}
