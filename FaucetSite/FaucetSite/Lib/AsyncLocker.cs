using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    /// <summary>
    /// Lock statement doesn't support async/await calls 
    /// so this is simple wrapper around <see cref="SemaphoreSlim"/> class to handle async/await calls by locking inside a using statement.
    /// For example: 'using (await locker.LockAsync()) { }'
    /// </summary>
    public class AsyncLocker
    {
        readonly SemaphoreSlim semaphore;

        public AsyncLocker(int maxDegreeOfParallelism = 1)
        {
            semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
        }

        public async Task<IDisposable> LockAsync()
        {
            await semaphore.WaitAsync();
            return new Disposable(semaphore);
        }

        private class Disposable : IDisposable
        {
            readonly SemaphoreSlim semaphore;
            public Disposable(SemaphoreSlim semaphore)
            {
                this.semaphore = semaphore;
            }
            public void Dispose()
            {
                semaphore.Release();
            }
        }
    }
}
