// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundSemaphorePublisher<T> : BusPublisher<T>
    {
        private readonly ConcurrentQueue<MessageContainer> publications = new ConcurrentQueue<MessageContainer>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly Func<IEnumerable<ISubscription<T>>, T, Task> publishMethod;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public BackgroundSemaphorePublisher(int concurrencyLevel = -1, BusPublisher<T> innerPublisher = null)
        {
            innerPublisher = innerPublisher ?? ParallelPublisher<T>.Default;

            this.publishMethod = innerPublisher.PublishAsync;

            if (concurrencyLevel < 1)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

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

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private async Task PublishWorkerAsync()
        {
            while (this.cancellationTokenSource.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(this.cancellationTokenSource.Token).ConfigureAwait(false);

                if (this.publications.TryDequeue(out var message))
                {
                    await this.publishMethod(message.Subscriptions, message.Message).ConfigureAwait(false);
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