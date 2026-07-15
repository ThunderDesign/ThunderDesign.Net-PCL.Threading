using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IGateKeeper : IDisposable
    {
        bool IsGateOpen { get; }

        long LockedGateableThreadLocks { get; }

        void OpenGate();

        Task CloseGateAsync(CancellationToken cancellationToken = default);
    }

    public interface IGateKeeper<TKey, TValue> : IGateKeeper where TKey : notnull where TValue : IGatedThreadLock
    {
        TValue GetOrAdd(TKey key);

        void Remove(TKey key);
    }
}
