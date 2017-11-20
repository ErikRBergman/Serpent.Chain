namespace Serpent.MessageHandlerChain.Decorators.Concurrent
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Models;

    internal class ConcurrentDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly ConcurrentQueue<MessageAndCompletionContainer<TMessageType>> messages = new ConcurrentQueue<MessageAndCompletionContainer<TMessageType>>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public ConcurrentDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int concurrencyLevel = -1)
        {
            if (concurrencyLevel <= 0)
            {
                concurrencyLevel = Environment.ProcessorCount * 2;
            }

            this.handlerFunc = handlerFunc;

            for (var i = 0; i < concurrencyLevel; i++)
            {
                Task.Run(this.MessageHandlerWorkerAsync);
            }
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            var taskCompletionSource = new TaskCompletionSource<TMessageType>();
            this.messages.Enqueue(new MessageAndCompletionContainer<TMessageType>(message, taskCompletionSource, token));
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

                if (this.messages.TryDequeue(out var message))
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
            }
        }
    }
}