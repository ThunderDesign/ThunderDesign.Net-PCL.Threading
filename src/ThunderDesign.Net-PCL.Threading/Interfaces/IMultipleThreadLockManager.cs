using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThunderDesign.Net.Threading.Interfaces
{
    public interface IMultipleThreadLockManager<TKey, TValue> : IDisposable where TKey : notnull
    {
        Task LockAsync(TKey key, CancellationToken cancellationToken = default);

        void Unlock(TKey key);
    }
}
