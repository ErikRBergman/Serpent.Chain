namespace Serpent.Common.MessageBus.SubscriptionTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class LimitedThroughputSubscription<TMessageType> : BusSubscription<TMessageType>
    {
        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly BusSubscription<TMessageType> innerSubscription;

        private readonly int maxMessagesPerPeriod;

        private readonly TimeSpan periodSpan;

        private readonly ConcurrentQueue<TMessageType> messages = new ConcurrentQueue<TMessageType>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public LimitedThroughputSubscription(BusSubscription<TMessageType> innerSubscription, int maxMessagesPerPeriod, TimeSpan periodSpan)
        {
            this.innerSubscription = innerSubscription;
            this.maxMessagesPerPeriod = maxMessagesPerPeriod;
            this.periodSpan = periodSpan;
            this.handlerFunc = innerSubscription.HandleMessageAsync;

            Task.Run(this.MessageHandlerWorkerAsync);
        }

        public LimitedThroughputSubscription(Func<TMessageType, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan periodSpan)
        {
            this.handlerFunc = handlerFunc;
            this.maxMessagesPerPeriod = maxMessagesPerPeriod;
            this.periodSpan = periodSpan;

            Task.Run(this.MessageHandlerWorkerAsync);
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            this.messages.Enqueue(message);
            this.semaphore.Release();
            return Task.CompletedTask;
        }

        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            DateTime periodStart = DateTime.Now;
            var periodMessageCount = 0;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token);

                if (periodMessageCount >= this.maxMessagesPerPeriod)
                {
                    var now = DateTime.Now;

                    var diff = (periodStart + this.periodSpan) - now;

                    // We will have to await the next period start
                    if (diff > TimeSpan.Zero)
                    {
                        await Task.Delay(diff, token);
                    }

                    periodStart = DateTime.Now;
                    periodMessageCount = 0;
                }

                if (this.messages.TryDequeue(out var message))
                {
                    periodMessageCount++;

                    try
                    {
                        await this.handlerFunc(message);
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