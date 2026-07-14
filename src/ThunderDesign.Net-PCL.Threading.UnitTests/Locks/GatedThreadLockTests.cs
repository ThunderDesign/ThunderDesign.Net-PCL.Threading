using System;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class GatedThreadLockTests
    {
        [Fact]
        public async Task LockAsync_ShouldThrowInvalidOperationException_WhenNotLinkedToGateKeeper()
        {
            using var gatedLock = new GatedThreadLock();

            await Assert.ThrowsAsync<InvalidOperationException>(() => gatedLock.LockAsync());
        }

        [Fact]
        public async Task LockAsync_ShouldSucceed_WhenLinkedViaGateKeeper()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            Assert.True(gatedLock.IsLocked);
            Assert.Equal(1, gateKeeper.LockedGateableThreadLocks);
        }

        [Fact]
        public async Task Unlock_ShouldReleaseLockAndExitGate()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();
            gatedLock.Unlock();

            Assert.False(gatedLock.IsLocked);
            Assert.Equal(0, gateKeeper.LockedGateableThreadLocks);
        }

        [Fact]
        public async Task LockAsync_ShouldBlockSecondCaller_ForSameGatedLock_UntilUnlocked()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            var secondLockTask = gatedLock.LockAsync();

            var completedEarly = await Task.WhenAny(secondLockTask, Task.Delay(200)) == secondLockTask;
            Assert.False(completedEarly);

            gatedLock.Unlock();

            var completed = await Task.WhenAny(secondLockTask, Task.Delay(1000)) == secondLockTask;
            Assert.True(completed);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowOperationCanceledException_WhenCancelled()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            using var cts = new CancellationTokenSource();
            var waitingTask = gatedLock.LockAsync(cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenLockDisposedWhileWaiting()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            var waitingTask = gatedLock.LockAsync();

            gatedLock.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldBeBlocked_WhenGateIsClosed()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            // Gate starts open, close it (no locks currently held so this returns immediately)
            await gateKeeper.CloseGateAsync();

            var lockTask = gatedLock.LockAsync();

            var completedEarly = await Task.WhenAny(lockTask, Task.Delay(200)) == lockTask;
            Assert.False(completedEarly);

            gateKeeper.OpenGate();

            var completed = await Task.WhenAny(lockTask, Task.Delay(1000)) == lockTask;
            Assert.True(completed);
        }
    }
}
