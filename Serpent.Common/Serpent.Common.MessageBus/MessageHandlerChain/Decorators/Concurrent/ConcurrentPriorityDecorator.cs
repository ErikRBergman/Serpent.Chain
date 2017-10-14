namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Models;

    internal class ConcurrentPriorityDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> prioritySelector;

        private readonly ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> highPriorityMessages = new ConcurrentQueue<MessageAndCompletionContainer<TMessageType>>();

        private readonly ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> lowPriorityMessages = new ConcurrentQueue<MessageAndCompletionContainer<TMessageType>>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public ConcurrentPriorityDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, Func<TMessageType, bool> prioritySelector, int concurrencyLevel = -1)
        {
            if (concurrencyLevel <= 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.handlerFunc = handlerFunc;
            this.prioritySelector = prioritySelector;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var taskCompletionSource = new TaskCompletionSource<TMessageType>();

            ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> messages;

            if (this.prioritySelector(message))
            {
                messages = this.highPriorityMessages;
            }
            else
            {
                messages = this.lowPriorityMessages;
            }

            messages.Enqueue(new MessageAndCompletionContainer<TMessageType>(message, taskCompletionSource, token));
            this.semaphore.Release();
            return taskCompletionSource.Task;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private async Task MessageHandlerWorkerAsync()
        {
            var token = this.cancellationTokenSource.Token;

            while (token.IsCancellationRequested == false)
            {
                await this.semaphore.WaitAsync(token).ConfigureAwait(false);

                MessageAndCompletionContainer<TMessageType> message;

                if (this.highPriorityMessages.TryDequeue(out message) == false)
                {
                    if (this.lowPriorityMessages.TryDequeue(out message) == false)
                    {
                        continue;
                    }
                }

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
        }
    }
}