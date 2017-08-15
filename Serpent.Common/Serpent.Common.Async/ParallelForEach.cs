namespace Serpent.Common.Async
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public static class EnumerableExtensions
    {
        private static readonly int ProcessorCount = Environment.ProcessorCount;

        public static Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> workerFunc, int concurrencyLevel)
        {
            return ForEachAsync(items, workerFunc, concurrencyLevel, CancellationToken.None);
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> workerFunc)
        {
            return ForEachAsync(items, workerFunc, ProcessorCount * 4, CancellationToken.None);
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> workerFunc, int concurrencyLevel, CancellationToken cancellationToken)
        {
            if (concurrencyLevel < 1)
            {
                throw new ArgumentException("concurrencyCount may not be less than 1");
            }

            var queue = new ConcurrentQueue<T>(items);

            return Task.WhenAll(Enumerable.Range(0, concurrencyLevel).Select(v => Task.Run(() => ForEachAsyncWorker(queue, workerFunc, cancellationToken), cancellationToken)));
        }

        public static Task ForEachAsync<TItem, TContext>(this IEnumerable<TItem> items, TContext context, Func<TItem, TContext, CancellationToken, Task> workerFunc, int concurrencyLevel, CancellationToken cancellationToken)
        {
            if (concurrencyLevel < 1)
            {
                throw new ArgumentException("concurrencyCount may not be less than 1");
            }

            var queue = new ConcurrentQueue<TItem>(items);

            return Task.WhenAll(Enumerable.Range(0, concurrencyLevel).Select(v => Task.Run(() => ForEachAsyncWorker(queue, context, workerFunc, cancellationToken), cancellationToken)));
        }

        private static async Task ForEachAsyncWorker<T>(ConcurrentQueue<T> queue, Func<T, Task> workerFunc, CancellationToken cancellationToken)
        {
            while (queue.TryDequeue(out var queueItem))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await workerFunc(queueItem).ConfigureAwait(false);
            }
        }

        private static async Task ForEachAsyncWorker<TItem, TContext>(ConcurrentQueue<TItem> queue, TContext context, Func<TItem, TContext, CancellationToken, Task> workerFunc, CancellationToken cancellationToken)
        {
            while (queue.TryDequeue(out var queueItem))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await workerFunc(queueItem, context, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}