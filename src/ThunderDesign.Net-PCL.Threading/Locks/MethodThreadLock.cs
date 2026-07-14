using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Locks
{
    public class MethodThreadLock : MultipleThreadLockManager<string, SingleThreadLock>, IMethodThreadLock, IDisposable
    {
        public override async Task LockAsync([CallerMemberName] string? methodName = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            await base.LockAsync(methodName!, cancellationToken).ConfigureAwait(false);
        }

        public override void Unlock([CallerMemberName] string? methodName = null)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            base.Unlock(methodName!);
        }
    }
}
