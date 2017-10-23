namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.First
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FirstAsyncDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate;

        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private IMessageBusSubscription subscription;

        private int wasReceived;

        public FirstAsyncDecorator(
            Func<TMessageType, CancellationToken, Task> handlerFunc,
            Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate,
            MessageHandlerChainBuilderSetupServices subscriptionServices)
        {
            this.handlerFunc = handlerFunc;
            this.asyncPredicate = asyncPredicate;
            subscriptionServices.SubscriptionNotification.AddNotification(this.SetSubscription);
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
                            this.subscription?.Dispose();
                        }
                    }
                }
            }
        }

        private void SetSubscription(IMessageBusSubscription sub)
        {
            this.subscription = sub;
        }
    }
}