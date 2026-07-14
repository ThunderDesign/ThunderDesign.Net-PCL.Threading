using System;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class SingleThreadLockTests
    {
        [Fact]
        public void IsLocked_ShouldBeFalse_WhenNotLocked()
        {
            using var threadLock = new SingleThreadLock();

            Assert.False(threadLock.IsLocked);
        }

        [Fact]
        public async Task LockAsync_ShouldSetIsLocked_WhenLockAcquired()
        {
            using var threadLock = new SingleThreadLock();

            await threadLock.LockAsync();

            Assert.True(threadLock.IsLocked);
        }

        [Fact]
        public async Task Unlock_ShouldReleaseLock_AllowingSubsequentLock()
        {
            using var threadLock = new SingleThreadLock();

            await threadLock.LockAsync();
            threadLock.Unlock();

            Assert.False(threadLock.IsLocked);

            // Should be able to lock again without blocking
            var task = threadLock.LockAsync();
            var completed = await Task.WhenAny(task, Task.Delay(1000)) == task;

            Assert.True(completed);
            Assert.True(threadLock.IsLocked);
        }

        [Fact]
        public void Unlock_ShouldThrow_WhenNotCurrentlyLocked()
        {
            using var threadLock = new SingleThreadLock();

            Assert.Throws<SynchronizationLockException>(() => threadLock.Unlock());
        }

        [Fact]
        public async Task SecondLockAsync_ShouldWait_UntilFirstIsUnlocked()
        {
            using var threadLock = new SingleThreadLock();

            await threadLock.LockAsync();

            var secondLockTask = threadLock.LockAsync();

            // The second lock should not complete while the first still holds the lock
            var completedEarly = await Task.WhenAny(secondLockTask, Task.Delay(200)) == secondLockTask;
            Assert.False(completedEarly);

            threadLock.Unlock();

            // Now the second lock should complete
            var completed = await Task.WhenAny(secondLockTask, Task.Delay(1000)) == secondLockTask;
            Assert.True(completed);
            Assert.True(threadLock.IsLocked);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowOperationCanceledException_WhenCancellationTokenIsCancelled()
        {
            using var threadLock = new SingleThreadLock();

            await threadLock.LockAsync();

            using var cts = new CancellationTokenSource();
            var waitingTask = threadLock.LockAsync(cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenLockIsDisposedWhileWaiting()
        {
            var threadLock = new SingleThreadLock();

            await threadLock.LockAsync();

            var waitingTask = threadLock.LockAsync();

            threadLock.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var threadLock = new SingleThreadLock();
            threadLock.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => threadLock.LockAsync());
        }

        [Fact]
        public void Unlock_ShouldThrowObjectDisposedException_WhenAlreadyDisposed()
        {
            var threadLock = new SingleThreadLock();
            threadLock.Dispose();

            Assert.Throws<ObjectDisposedException>(() => threadLock.Unlock());
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var threadLock = new SingleThreadLock();

            threadLock.Dispose();
            var exception = Record.Exception(() => threadLock.Dispose());

            Assert.Null(exception);
        }

        [Fact]
        public async Task LockAsync_ShouldOnlyAllowOneThread_UnderConcurrentAccess()
        {
            using var threadLock = new SingleThreadLock();
            var counter = 0;
            var maxObservedConcurrency = 0;
            var gate = new object();

            async Task Worker()
            {
                await threadLock.LockAsync();
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
                    threadLock.Unlock();
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
