namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    public class BackgroundSemaphoreDuplicateEliminatingSubscription<TMessageType, TKeyType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly BusSubscription<TMessageType> innerSubscription;

        private readonly Func<TMessageType, TKeyType> keySelector;

        private readonly ConcurrentDictionary<TKeyType, bool> keyDictionary = new ConcurrentDictionary<TKeyType, bool>();

        private readonly ConcurrentQueue<MessageAndKey> messages = new ConcurrentQueue<MessageAndKey>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private struct MessageAndKey
        {
            public MessageAndKey(TKeyType key, TMessageType message)
            {
                this.Key = key;
                this.Message = message;
            }

            public TMessageType Message { get; }

            public TKeyType Key { get; }
        }

        public BackgroundSemaphoreDuplicateEliminatingSubscription(BusSubscription<TMessageType> innerSubscription, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            if (concurrencyLevel < 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.innerSubscription = innerSubscription;
            this.handlerFunc = innerSubscription.HandleMessageAsync;
            this.keySelector = keySelector;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public BackgroundSemaphoreDuplicateEliminatingSubscription(Func<TMessageType, Task> handlerFunc, Func<TMessageType, TKeyType> keySelector, int concurrencyLevel = -1)
        {
            if (concurrencyLevel < 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.handlerFunc = handlerFunc;
            this.keySelector = keySelector;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            var key = this.keySelector(message);

            if (this.keyDictionary.TryAdd(key, true))
            {
                this.messages.Enqueue(new MessageAndKey(key, message));
                this.semaphore.Release();
            }

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
                    try
                    {
                        await this.handlerFunc(message.Message);
                    }
                    catch (Exception)
                    {
                        // don't ruin the subscription when the user has not caught an exception
                    }

                    // Remove key after the message handler is invoked. The user can decorate with a fire and forget subscription to have the key removed before the handler is invoked.
                    this.keyDictionary.TryRemove(message.Key, out _);
                }
            }
        }
    }
}