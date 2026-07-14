using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class GateKeeperTests
    {
        [Fact]
        public void IsGateOpen_ShouldBeTrue_Initially()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            Assert.True(gateKeeper.IsGateOpen);
            Assert.Equal(0, gateKeeper.LockedGateableThreadLocks);
        }

        [Fact]
        public void GetOrAdd_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            Assert.Throws<ArgumentNullException>(() => gateKeeper.GetOrAdd(null!));
        }

        [Fact]
        public void GetOrAdd_ShouldReturnSameInstance_ForSameKey()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            var lock1 = gateKeeper.GetOrAdd("key1");
            var lock2 = gateKeeper.GetOrAdd("key1");

            Assert.Same(lock1, lock2);
        }

        [Fact]
        public void Remove_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            Assert.Throws<ArgumentNullException>(() => gateKeeper.Remove(null!));
        }

        [Fact]
        public void Remove_ShouldThrowKeyNotFoundException_WhenKeyDoesNotExist()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            Assert.Throws<KeyNotFoundException>(() => gateKeeper.Remove("missing-key"));
        }

        [Fact]
        public void Remove_ShouldSucceed_ForExistingKey()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            gateKeeper.GetOrAdd("key1");

            var exception = Record.Exception(() => gateKeeper.Remove("key1"));

            Assert.Null(exception);
        }

        [Fact]
        public void OpenGate_ShouldThrowInvalidOperationException_WhenGateAlreadyOpen()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            Assert.Throws<InvalidOperationException>(() => gateKeeper.OpenGate());
        }

        [Fact]
        public async Task CloseGateAsync_ShouldCompleteImmediately_WhenNoLocksHeld()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            await gateKeeper.CloseGateAsync();

            Assert.False(gateKeeper.IsGateOpen);
        }

        [Fact]
        public async Task CloseGateAsync_ShouldWait_UntilAllGatedLocksAreUnlocked()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            var closeGateTask = gateKeeper.CloseGateAsync();

            var completedEarly = await Task.WhenAny(closeGateTask, Task.Delay(200)) == closeGateTask;
            Assert.False(completedEarly);

            gatedLock.Unlock();

            var completed = await Task.WhenAny(closeGateTask, Task.Delay(1000)) == closeGateTask;
            Assert.True(completed);
            Assert.False(gateKeeper.IsGateOpen);
        }

        [Fact]
        public async Task OpenGate_ShouldAllowNewLocks_AfterGateWasClosed()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gateKeeper.CloseGateAsync();

            var lockTask = gatedLock.LockAsync();
            var completedEarly = await Task.WhenAny(lockTask, Task.Delay(200)) == lockTask;
            Assert.False(completedEarly);

            gateKeeper.OpenGate();

            Assert.True(gateKeeper.IsGateOpen);

            var completed = await Task.WhenAny(lockTask, Task.Delay(1000)) == lockTask;
            Assert.True(completed);
        }

        [Fact]
        public async Task CloseGateAsync_ShouldThrowOperationCanceledException_WhenCancelledWhileWaitingForLocks()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();

            using var cts = new CancellationTokenSource();
            var closeGateTask = gateKeeper.CloseGateAsync(cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => closeGateTask);

            gatedLock.Unlock();
        }

        [Fact]
        public async Task CloseGateAsync_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            gateKeeper.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => gateKeeper.CloseGateAsync());
        }

        [Fact]
        public void GetOrAdd_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            gateKeeper.Dispose();

            Assert.Throws<ObjectDisposedException>(() => gateKeeper.GetOrAdd("key1"));
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            gateKeeper.Dispose();
            var exception = Record.Exception(() => gateKeeper.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public async Task Dispose_ShouldDisposeAllGatedLocks()
        {
            var gateKeeper = new GateKeeper<string, GatedThreadLock>();
            var gatedLock = gateKeeper.GetOrAdd("key1");

            await gatedLock.LockAsync();
            gatedLock.Unlock();

            gateKeeper.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => gatedLock.LockAsync());
        }

        [Fact]
        public async Task MultipleGatedLocks_ShouldIndependentlyIncrementAndDecrement_LockedCount()
        {
            using var gateKeeper = new GateKeeper<string, GatedThreadLock>();

            var lock1 = gateKeeper.GetOrAdd("key1");
            var lock2 = gateKeeper.GetOrAdd("key2");

            await lock1.LockAsync();
            Assert.Equal(1, gateKeeper.LockedGateableThreadLocks);

            await lock2.LockAsync();
            Assert.Equal(2, gateKeeper.LockedGateableThreadLocks);

            lock1.Unlock();
            Assert.Equal(1, gateKeeper.LockedGateableThreadLocks);

            lock2.Unlock();
            Assert.Equal(0, gateKeeper.LockedGateableThreadLocks);
        }
    }
}
