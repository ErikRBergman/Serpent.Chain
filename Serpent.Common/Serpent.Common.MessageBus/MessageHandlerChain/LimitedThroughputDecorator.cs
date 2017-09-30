namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Models;

    public class LimitedThroughputDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, Task> handlerFunc;

        private readonly int maxMessagesPerPeriod;

        private readonly ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> messages = new ConcurrentQueue<MessageAndCompletionContainer<TMessageType>>();

        private readonly TimeSpan periodSpan;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public LimitedThroughputDecorator(Func<TMessageType, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan periodSpan)
        {
            this.handlerFunc = handlerFunc;
            this.maxMessagesPerPeriod = maxMessagesPerPeriod;
            this.periodSpan = periodSpan;

            Task.Run(this.MessageHandlerWorkerAsync);
        }

        public override Task HandleMessageAsync(TMessageType message)
        {
            var taskCompletionSource = new TaskCompletionSource<TMessageType>();
            this.messages.Enqueue(new MessageAndCompletionContainer<TMessageType>(message, taskCompletionSource));
            this.semaphore.Release();
            return taskCompletionSource.Task;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            var periodMessageCount = 0;
            var periodTimeSpan = this.periodSpan;

            var periodEnd = DateTime.UtcNow + periodTimeSpan;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token).ConfigureAwait(false);

                var diff = periodEnd - DateTime.UtcNow;
                if (diff < TimeSpan.Zero)
                {
                    periodEnd = DateTime.UtcNow + periodTimeSpan;
                    periodMessageCount = 0;
                }
                else if (periodMessageCount >= this.maxMessagesPerPeriod)
                {
                    // We will have to await the next period start
                    if (diff > TimeSpan.Zero)
                    {
                        await Task.Delay(diff, token).ConfigureAwait(false);
                    }

                    periodEnd = DateTime.UtcNow + periodTimeSpan;
                    periodMessageCount = 0;
                }

                if (this.messages.TryDequeue(out var message))
                {
                    periodMessageCount++;

                    // Do not await this call. If we did, a single slow call could ruin the throughput
#pragma warning disable 4014
                    this.DispatchMessageAsync(message);
#pragma warning restore 4014
                }
            }
        }

        private async Task DispatchMessageAsync(MessageAndCompletionContainer<TMessageType> message)
        {
            await Task.Yield();

            try
            {
                await this.handlerFunc(message.Message).ConfigureAwait(false);
                message.TaskCompletionSource.SetResult(message.Message);
            }
            catch (Exception exception)
            {
                message.TaskCompletionSource.SetException(exception);
            }
        }
    }
}