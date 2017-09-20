// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class SemaphorePublisher<T> : BusPublisher<T>
    {
        private readonly SemaphoreSlim semaphore;

        private Func<IEnumerable<ISubscription<T>>, T, Task> publishMethod;

        public SemaphorePublisher(int concurrencyLevel = -1, BusPublisher<T> innerPublisher = null)
        {
            innerPublisher = innerPublisher ?? ParallelPublisher<T>.Default;

            this.publishMethod = innerPublisher.PublishAsync;

            if (concurrencyLevel < 1)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.semaphore = new SemaphoreSlim(concurrencyLevel);
        }

        public override async Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message)
        {
            try
            {
                await this.semaphore.WaitAsync().ConfigureAwait(false);
                await this.publishMethod(subscriptions, message).ConfigureAwait(false);
            }
            finally
            {
                this.semaphore.Release();
            }
        }
    }
}