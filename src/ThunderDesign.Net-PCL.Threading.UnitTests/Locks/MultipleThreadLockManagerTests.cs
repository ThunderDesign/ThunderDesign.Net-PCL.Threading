using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class MultipleThreadLockManagerTests
    {
        [Fact]
        public async Task LockAsync_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            await Assert.ThrowsAsync<ArgumentNullException>(() => manager.LockAsync(null!));
        }

        [Fact]
        public void Unlock_ShouldThrowArgumentNullException_WhenKeyIsNull()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            Assert.Throws<ArgumentNullException>(() => manager.Unlock(null!));
        }

        [Fact]
        public void Unlock_ShouldThrowKeyNotFoundException_WhenKeyDoesNotExist()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            Assert.Throws<KeyNotFoundException>(() => manager.Unlock("missing-key"));
        }

        [Fact]
        public async Task Unlock_ShouldThrowSynchronizationLockException_WhenKeyLockIsNotHeld()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            await manager.LockAsync("key1");
            manager.Unlock("key1");

            Assert.Throws<SynchronizationLockException>(() => manager.Unlock("key1"));
        }

        [Fact]
        public async Task LockAsync_ShouldCreateSeparateLocks_ForDifferentKeys()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            var lockTask1 = manager.LockAsync("key1");
            var lockTask2 = manager.LockAsync("key2");

            var waitTask = Task.Delay(1000);
            var completed = await Task.WhenAny(Task.WhenAll(lockTask1, lockTask2), waitTask);

            Assert.NotEqual(waitTask, completed);
        }

        [Fact]
        public async Task LockAsync_ShouldBlock_ForSameKey_UntilUnlocked()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            await manager.LockAsync("shared-key");

            var secondLockTask = manager.LockAsync("shared-key");

            var completedEarly = await Task.WhenAny(secondLockTask, Task.Delay(200)) == secondLockTask;
            Assert.False(completedEarly);

            manager.Unlock("shared-key");

            var completed = await Task.WhenAny(secondLockTask, Task.Delay(1000)) == secondLockTask;
            Assert.True(completed);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowOperationCanceledException_WhenCancelled()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            await manager.LockAsync("key1");

            using var cts = new CancellationTokenSource();
            var waitingTask = manager.LockAsync("key1", cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenManagerIsDisposedWhileWaiting()
        {
            var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            await manager.LockAsync("key1");

            var waitingTask = manager.LockAsync("key1");

            manager.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var manager = new MultipleThreadLocks<string, SingleThreadLock>();
            manager.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => manager.LockAsync("key1"));
        }

        [Fact]
        public void Unlock_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var manager = new MultipleThreadLocks<string, SingleThreadLock>();
            manager.Dispose();

            Assert.Throws<ObjectDisposedException>(() => manager.Unlock("key1"));
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var manager = new MultipleThreadLocks<string, SingleThreadLock>();

            manager.Dispose();
            var exception = Record.Exception(() => manager.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public async Task LockAsync_ShouldOnlyAllowOneThreadPerKey_UnderConcurrentAccess()
        {
            using var manager = new MultipleThreadLocks<string, SingleThreadLock>();
            var counter = 0;
            var maxObservedConcurrency = 0;
            var gate = new object();

            async Task Worker()
            {
                await manager.LockAsync("shared-key");
                try
                {
                    lock (gate)
                    {
                        counter++;
                        maxObservedConcurrency = Math.Max(maxObservedConcurrency, counter);
                    }

                    await Task.Delay(20);

                    lock (gate)
                    {
                        counter--;
                    }
                }
                finally
                {
                    manager.Unlock("shared-key");
                }
            }

            var tasks = new Task[10];
            for (int i = 0; i < tasks.Length; i++)
                tasks[i] = Worker();

            await Task.WhenAll(tasks);

            Assert.Equal(1, maxObservedConcurrency);
        }
    }
}
