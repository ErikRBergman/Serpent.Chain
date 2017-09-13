namespace Serpent.Common.MessageBus.BusPublishers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundSemaphorePublisher<T> : BusPublisher<T>
    {
        private readonly ConcurrentQueue<MessageContainer> publications = new ConcurrentQueue<MessageContainer>();

        private readonly SemaphoreSlim semaphore;

        private Func<IEnumerable<ISubscription<T>>, T, Task> publishMethod;

        public BackgroundSemaphorePublisher(int concurrencyLevel = -1, BusPublisher<T> innerPublisher = null)
        {
            innerPublisher = innerPublisher ?? SerialPublisher<T>.Default;

            this.publishMethod = innerPublisher.PublishAsync;

            if (concurrencyLevel < 1)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.semaphore = new SemaphoreSlim(concurrencyLevel);

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.PublishWorkerAsync);
            }
        }

        public override Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message)
        {
            this.publications.Enqueue(new MessageContainer(subscriptions, message));
            this.semaphore.Release();
            return Task.CompletedTask;
            
        }

        private async Task PublishWorkerAsync()
        {
            while (true)
            {
                await this.semaphore.WaitAsync();

                if (this.publications.TryDequeue(out var message))
                {
                    await this.publishMethod(message.Subscriptions, message.Message);
                }
            }
        }

        private struct MessageContainer
        {
            public MessageContainer(IEnumerable<ISubscription<T>> subscriptions, T message)
            {
                this.Message = message;
                this.Subscriptions = subscriptions;
            }

            public T Message { get; set; }

            public IEnumerable<ISubscription<T>> Subscriptions { get; set; }
        }
    }
}