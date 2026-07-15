using System;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class ISingleThreadLockExtentions
    {
        public static async Task WrapWithLockAsync(this ISingleThreadLock threadLocks, Func<Task> action, CancellationToken cancellationToken = default)
        {
            if (threadLocks == null)
                throw new ArgumentNullException(nameof(threadLocks));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            await threadLocks.LockAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                threadLocks.Unlock();
            }
        }

        public static async Task<T> WrapWithLockAsync<T>(this ISingleThreadLock threadLocks, Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            if (threadLocks == null)
                throw new ArgumentNullException(nameof(threadLocks));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            await threadLocks.LockAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                threadLocks.Unlock();
            }
        }
    }
}
