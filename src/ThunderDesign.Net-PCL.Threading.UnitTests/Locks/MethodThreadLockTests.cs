using System;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class MethodThreadLockTests
    {
        private async Task MethodA(MethodThreadLock methodLock, int delayMs)
        {
            await methodLock.LockAsync();
            try
            {
                await Task.Delay(delayMs);
            }
            finally
            {
                methodLock.Unlock();
            }
        }

        private async Task MethodB(MethodThreadLock methodLock, int delayMs)
        {
            await methodLock.LockAsync();
            try
            {
                await Task.Delay(delayMs);
            }
            finally
            {
                methodLock.Unlock();
            }
        }

        [Fact]
        public async Task LockAsync_ShouldUseCallerMemberName_ToScopeLock()
        {
            using var methodLock = new MethodThreadLock();

            var firstCall = MethodA(methodLock, 200);

            await Task.Delay(20); // allow first call to acquire lock

            var secondCall = MethodA(methodLock, 20);

            var completedEarly = await Task.WhenAny(secondCall, Task.Delay(50)) == secondCall;
            Assert.False(completedEarly);

            await Task.WhenAll(firstCall, secondCall);
        }

        [Fact]
        public async Task LockAsync_ShouldNotBlock_DifferentMethods()
        {
            using var methodLock = new MethodThreadLock();

            var callA = MethodA(methodLock, 200);
            var callB = MethodB(methodLock, 200);

            var completed = await Task.WhenAny(Task.WhenAll(callA, callB), Task.Delay(500));

            Assert.Equal(Task.WhenAll(callA, callB).IsCompletedSuccessfully, completed.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task LockAsync_WithExplicitMethodName_ShouldScopeIndependently()
        {
            using var methodLock = new MethodThreadLock();

            await methodLock.LockAsync("CustomKey1");

            var otherKeyTask = methodLock.LockAsync("CustomKey2");
            var completed = await Task.WhenAny(otherKeyTask, Task.Delay(500)) == otherKeyTask;

            Assert.True(completed);

            methodLock.Unlock("CustomKey1");
            methodLock.Unlock("CustomKey2");
        }

        [Fact]
        public async Task LockAsync_ShouldThrowArgumentNullException_WhenMethodNameIsNullOrWhiteSpace()
        {
            using var methodLock = new MethodThreadLock();

            await Assert.ThrowsAsync<ArgumentNullException>(() => methodLock.LockAsync("   "));
        }

        [Fact]
        public void Unlock_ShouldThrowArgumentNullException_WhenMethodNameIsNullOrWhiteSpace()
        {
            using var methodLock = new MethodThreadLock();

            Assert.Throws<ArgumentNullException>(() => methodLock.Unlock("   "));
        }

        [Fact]
        public async Task LockAsync_ShouldThrowOperationCanceledException_WhenCancelled()
        {
            using var methodLock = new MethodThreadLock();

            await methodLock.LockAsync(nameof(LockAsync_ShouldThrowOperationCanceledException_WhenCancelled));

            using var cts = new CancellationTokenSource();
            var waitingTask = methodLock.LockAsync(nameof(LockAsync_ShouldThrowOperationCanceledException_WhenCancelled), cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenDisposedWhileWaiting()
        {
            var methodLock = new MethodThreadLock();

            await methodLock.LockAsync("SharedMethod");

            var waitingTask = methodLock.LockAsync("SharedMethod");
            methodLock.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldOnlyAllowOneCaller_PerMethodName_UnderConcurrentAccess()
        {
            using var methodLock = new MethodThreadLock();
            var counter = 0;
            var maxObservedConcurrency = 0;
            var gate = new object();

            async Task Worker()
            {
                await methodLock.LockAsync(nameof(Worker));
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
                    methodLock.Unlock(nameof(Worker));
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
