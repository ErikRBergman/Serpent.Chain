namespace Serpent.Common.Async
{
    using System;
    using System.Threading;

    public struct SemaphoreSlimReleaser : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim;

        public SemaphoreSlimReleaser(SemaphoreSlim semaphoreSlim)
        {
            this.semaphoreSlim = semaphoreSlim;
        }

        public void Dispose()
        {
            this.semaphoreSlim.Release();
        }
    }
}