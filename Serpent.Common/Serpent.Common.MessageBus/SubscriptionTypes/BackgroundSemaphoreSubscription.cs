namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundSemaphoreSubscription<T> : BusSubscription<T>
    {
        private readonly Func<T, Task> handlerFunc;

        private readonly BusSubscription<T> innerSubscription;

        private readonly ConcurrentQueue<T> messages = new ConcurrentQueue<T>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public BackgroundSemaphoreSubscription(BusSubscription<T> innerSubscription, int concurrencyLevel = -1)
        {
            if (concurrencyLevel < 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.innerSubscription = innerSubscription;
            this.handlerFunc = innerSubscription.HandleMessageAsync;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public BackgroundSemaphoreSubscription(Func<T, Task> handlerFunc, int concurrencyLevel = -1)
        {
            if (concurrencyLevel < 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.handlerFunc = handlerFunc;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public override Task HandleMessageAsync(T message)
        {
            this.messages.Enqueue(message);
            this.semaphore.Release();
            return Task.CompletedTask;
        }

        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token);

                if (this.messages.TryDequeue(out var message))
                {
                    await this.handlerFunc(message);
                }
            }
        }
    }
}