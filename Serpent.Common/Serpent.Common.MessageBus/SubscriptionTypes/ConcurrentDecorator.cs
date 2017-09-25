namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    public class ConcurrentDecorator<TMessageType> : MessageHandlerDecorator<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly ConcurrentQueue<TMessageType> messages = new ConcurrentQueue<TMessageType>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public ConcurrentDecorator(Func<TMessageType, Task> handlerFunc, int concurrencyLevel = -1)
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

        public override Task HandleMessageAsync(TMessageType message)
        {
            this.messages.Enqueue(message);
            this.semaphore.Release();
            return Task.CompletedTask;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token).ConfigureAwait(false);

                if (this.messages.TryDequeue(out var message))
                {
                    try
                    {
                        await this.handlerFunc(message).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        // don't ruin the subscription when the user has not caught an exception
                    }
                }
            }
        }
    }
}