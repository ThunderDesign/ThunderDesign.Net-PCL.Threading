using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface ISingleThreadLock : IDisposable
    {
        bool IsLocked { get; }
        Task LockAsync(CancellationToken cancellationToken = default);
        void Unlock();
    }
}
