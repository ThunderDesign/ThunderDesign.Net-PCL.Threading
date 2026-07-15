using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Interfaces;

namespace ThunderDesign.Net.Threading.Extentions
{
    public static class IMethodThreadLocksExtentions
    {
        public static async Task WrapWithLockAsync(this IMethodThreadLocks methodThreadLocks, Func<Task> action, [CallerMemberName] string? methodName = null, CancellationToken cancellationToken = default)
        {
            if (methodThreadLocks == null)
                throw new ArgumentNullException(nameof(methodThreadLocks));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            await methodThreadLocks.LockAsync(methodName, cancellationToken).ConfigureAwait(false);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                methodThreadLocks.Unlock(methodName);
            }
        }

        public static async Task<T> WrapWithLockAsync<T>(this IMethodThreadLocks methodThreadLocks, Func<Task<T>> action, [CallerMemberName] string? methodName = null, CancellationToken cancellationToken = default)
        {
            if (methodThreadLocks == null)
                throw new ArgumentNullException(nameof(methodThreadLocks));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException(nameof(methodName));

            await methodThreadLocks.LockAsync(methodName, cancellationToken).ConfigureAwait(false);
            try
            {
                return await action().ConfigureAwait(false);
            }
            finally
            {
                methodThreadLocks.Unlock(methodName);
            }
        }
    }
}
