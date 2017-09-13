namespace Serpent.Common.Async
{
    using System.Threading;
    using System.Threading.Tasks;

    public static class SemaphoreSlimExtensions
    {
        public static async Task<SemaphoreSlimReleaser> LockAsync(this SemaphoreSlim semaphoreSlim)
        {
            await semaphoreSlim.WaitAsync();
            return new SemaphoreSlimReleaser(semaphoreSlim);
        }
    }
}