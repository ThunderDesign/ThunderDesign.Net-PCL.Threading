using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IMethodThreadLocks : IDisposable
    {
        Task LockAsync([CallerMemberName] string? methodName = null, CancellationToken cancellationToken = default);

        void Unlock([CallerMemberName] string? methodName = null);
    }
}
