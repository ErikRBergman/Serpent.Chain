namespace Serpent.MessageBus.MessageHandlerChain.Decorators.First
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FirstAsyncDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate;

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private IMessageHandlerChain messageHandlerChain;

        private int wasReceived;

        public FirstAsyncDecorator(
            Func<TMessageType, CancellationToken, Task> handlerFunc,
            Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate,
            MessageHandlerChainBuilderSetupServices subscriptionServices)
        {
            this.handlerFunc = handlerFunc;
            this.asyncPredicate = asyncPredicate;
            subscriptionServices.BuildNotification.AddNotification(this.MessageHandlerChainBuilt);
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.wasReceived == 0)
            {
                if (await this.asyncPredicate(message, token).ConfigureAwait(false))
                {
                    if (Interlocked.CompareExchange(ref this.wasReceived, 1, 0) == 0)
                    {
                        try
                        {
                            await this.handlerFunc(message, token).ConfigureAwait(false);
                        }
                        finally
                        {
                            this.messageHandlerChain?.Dispose();
                        }
                    }
                }
            }
        }

        private void MessageHandlerChainBuilt(IMessageHandlerChain messageHandlerChain)
        {
            this.messageHandlerChain = messageHandlerChain;
        }
    }
}