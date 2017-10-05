namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Concurrent
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;

    public class ConcurrentFireAndForgetDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly ConcurrentQueue<MessageAndToken> messages = new ConcurrentQueue<MessageAndToken>();

        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(0);

        public ConcurrentFireAndForgetDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, int concurrencyLevel = -1)
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

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            this.messages.Enqueue(new MessageAndToken(message, token));
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
                        await this.handlerFunc(message.Message, message.Token).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        // don't ruin the subscription when the user has not caught an exception
                    }
                }
            }
        }

        private struct MessageAndToken
        {
            public MessageAndToken(TMessageType message, CancellationToken token)
            {
                this.Message = message;
                this.Token = token;
            }

            public TMessageType Message { get; }

            public CancellationToken Token { get; }
        }
    }
}