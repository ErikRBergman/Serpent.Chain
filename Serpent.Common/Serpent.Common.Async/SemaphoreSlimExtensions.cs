using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.Common.Async
{
    using System.Threading;

    public static class SemaphoreSlimExtensions
    {
        public static async Task<SemaphoreSlimReleaser> LockAsync(this SemaphoreSlim semaphoreSlim)
        {
            await semaphoreSlim.WaitAsync();
            return new SemaphoreSlimReleaser(semaphoreSlim);
        }

    }
}
