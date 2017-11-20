namespace Serpent.MessageHandlerChain.Decorators.LimitedThroughput
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Models;

    /// <summary>
    /// The limited throughput message handler chain decorator
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    internal class LimitedThroughputDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly int maxMessagesPerPeriod;

        private readonly ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> messages = new ConcurrentQueue<MessageAndCompletionContainer<TMessageType>>();

        private readonly TimeSpan periodSpan;

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handlerFunc">The handler func (or inner decorator)</param>
        /// <param name="maxMessagesPerPeriod">The maximum number of messages to handle over the period</param>
        /// <param name="periodSpan">The period span</param>
        public LimitedThroughputDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int maxMessagesPerPeriod, TimeSpan periodSpan)
        {
            this.handlerFunc = handlerFunc;
            this.maxMessagesPerPeriod = maxMessagesPerPeriod;
            this.periodSpan = periodSpan;

            Task.Run(this.MessageHandlerWorkerAsync);
        }

        /// <summary>
        /// The message handler
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="token">the cancellation token</param>
        /// <returns>A task that succeeds when the message is handled</returns>
        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var taskCompletionSource = new TaskCompletionSource<TMessageType>();
            this.messages.Enqueue(new MessageAndCompletionContainer<TMessageType>(message, taskCompletionSource, token));
            this.semaphore.Release();
            return taskCompletionSource.Task;
        }

        private async Task DispatchMessageAsync(MessageAndCompletionContainer<TMessageType> message)
        {
            try
            {
                await this.handlerFunc(message.Message, message.CancellationToken).ConfigureAwait(false);
                message.TaskCompletionSource.SetResult(message.Message);
            }
            catch (Exception exception)
            {
                message.TaskCompletionSource.SetException(exception);
            }
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
    }
}