using System;
using System.Threading;
using System.Threading.Tasks;
using ThunderDesign.Net.Threading.Locks;
using Xunit;

namespace ThunderDesign.Net_PCL.Threading.UnitTests.Locks
{
    public class MethodThreadLocksTests
    {
        private async Task MethodA(MethodThreadLocks methodLocks, int delayMs)
        {
            await methodLocks.LockAsync();
            try
            {
                await Task.Delay(delayMs);
            }
            finally
            {
                methodLocks.Unlock();
            }
        }

        private async Task MethodB(MethodThreadLocks methodLocks, int delayMs)
        {
            await methodLocks.LockAsync();
            try
            {
                await Task.Delay(delayMs);
            }
            finally
            {
                methodLocks.Unlock();
            }
        }

        [Fact]
        public async Task LockAsync_ShouldUseCallerMemberName_ToScopeLock()
        {
            using var methodLocks = new MethodThreadLocks();

            var firstCall = MethodA(methodLocks, 200);

            await Task.Delay(20); // allow first call to acquire lock

            var secondCall = MethodA(methodLocks, 20);

            var completedEarly = await Task.WhenAny(secondCall, Task.Delay(50)) == secondCall;
            Assert.False(completedEarly);

            await Task.WhenAll(firstCall, secondCall);
        }

        [Fact]
        public async Task LockAsync_ShouldNotBlock_DifferentMethods()
        {
            using var methodLocks = new MethodThreadLocks();

            var callA = MethodA(methodLocks, 200);
            var callB = MethodB(methodLocks, 200);

            var completed = await Task.WhenAny(Task.WhenAll(callA, callB), Task.Delay(500));

            Assert.Equal(Task.WhenAll(callA, callB).IsCompletedSuccessfully, completed.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task LockAsync_WithExplicitMethodName_ShouldScopeIndependently()
        {
            using var methodLocks = new MethodThreadLocks();

            await methodLocks.LockAsync("CustomKey1");

            var otherKeyTask = methodLocks.LockAsync("CustomKey2");
            var completed = await Task.WhenAny(otherKeyTask, Task.Delay(500)) == otherKeyTask;

            Assert.True(completed);

            methodLocks.Unlock("CustomKey1");
            methodLocks.Unlock("CustomKey2");
        }

        [Fact]
        public async Task LockAsync_ShouldThrowArgumentNullException_WhenMethodNameIsNullOrWhiteSpace()
        {
            using var methodLocks = new MethodThreadLocks();

            await Assert.ThrowsAsync<ArgumentNullException>(() => methodLocks.LockAsync("   "));
        }

        [Fact]
        public void Unlock_ShouldThrowArgumentNullException_WhenMethodNameIsNullOrWhiteSpace()
        {
            using var methodLocks = new MethodThreadLocks();

            Assert.Throws<ArgumentNullException>(() => methodLocks.Unlock("   "));
        }

        [Fact]
        public async Task LockAsync_ShouldThrowOperationCanceledException_WhenCancelled()
        {
            using var methodLocks = new MethodThreadLocks();

            await methodLocks.LockAsync(nameof(LockAsync_ShouldThrowOperationCanceledException_WhenCancelled));

            using var cts = new CancellationTokenSource();
            var waitingTask = methodLocks.LockAsync(nameof(LockAsync_ShouldThrowOperationCanceledException_WhenCancelled), cts.Token);

            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldThrowObjectDisposedException_WhenDisposedWhileWaiting()
        {
            var methodLocks = new MethodThreadLocks();

            await methodLocks.LockAsync("SharedMethod");

            var waitingTask = methodLocks.LockAsync("SharedMethod");
            methodLocks.Dispose();

            await Assert.ThrowsAsync<ObjectDisposedException>(() => waitingTask);
        }

        [Fact]
        public async Task LockAsync_ShouldOnlyAllowOneCaller_PerMethodName_UnderConcurrentAccess()
        {
            using var methodLocks = new MethodThreadLocks();
            var counter = 0;
            var maxObservedConcurrency = 0;
            var gate = new object();

            async Task Worker()
            {
                await methodLocks.LockAsync(nameof(Worker));
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
                    methodLocks.Unlock(nameof(Worker));
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
