namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    public class LimitedThroughputFireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxMessagesPerPeriod;

        private readonly ConcurrentQueue<TMessageType> messages = new ConcurrentQueue<TMessageType>();

        private readonly TimeSpan periodSpan;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public LimitedThroughputFireAndForgetDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan periodSpan)
        {
            this.handlerFunc = handlerFunc;
            this.maxMessagesPerPeriod = maxMessagesPerPeriod;
            this.periodSpan = periodSpan;

            Task.Run(this.MessageHandlerWorkerAsync);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            this.messages.Enqueue(message);
            this.semaphore.Release();
            return Task.CompletedTask;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            var periodStart = DateTime.UtcNow;
            var periodMessageCount = 0;
            var periodTimeSpan = this.periodSpan;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token).ConfigureAwait(false);

                var diff = (periodStart + periodTimeSpan) - DateTime.UtcNow;
                if (diff < TimeSpan.Zero)
                {
                    periodStart = DateTime.UtcNow;
                    periodMessageCount = 0;
                }
                else if (periodMessageCount >= this.maxMessagesPerPeriod)
                {
                    // We will have to await the next period start
                    if (diff > TimeSpan.Zero)
                    {
                        await Task.Delay(diff, token).ConfigureAwait(false);
                    }

                    periodStart = DateTime.UtcNow;
                    periodMessageCount = 0;
                }

                if (this.messages.TryDequeue(out var message))
                {
                    periodMessageCount++;

                    try
                    {
#pragma warning disable 4014
                        Task.Run(() => this.handlerFunc(message,  token));
#pragma warning restore 4014
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