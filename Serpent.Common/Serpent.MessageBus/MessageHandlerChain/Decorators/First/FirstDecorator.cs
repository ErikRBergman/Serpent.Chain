namespace Serpent.MessageBus.MessageHandlerChain.Decorators.First
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class FirstDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>
    {
        private readonly Func<TMessageType, CancellationToken, Task> handlerFunc;

        private readonly Func<TMessageType, bool> predicate;

        private IMessageHandlerChain messageHandlerChain;

        private int wasReceived;

        public FirstDecorator(
            Func<TMessageType, CancellationToken, Task> handlerFunc,
            Func<TMessageType, bool> predicate,
            MessageHandlerChainBuilderSetupServices subscriptionServices)
        {
            this.handlerFunc = handlerFunc;
            this.predicate = predicate;
            subscriptionServices.BuildNotification.AddNotification(this.MessageHandlerChainBuilt);
        }

        public override async Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.wasReceived == 0)
            {
                if (this.predicate(message))
                {
                    if (Interlocked.CompareExchange(ref this.wasReceived, 1, 0) == 0)
                    {
                        try
                        {
                            await this.handlerFunc(message, token);
                        }
                        finally
                        {
                            this.messageHandlerChain?.Dispose();
                        }
                    }
                }
            }
        }

        private void MessageHandlerChainBuilt(IMessageHandlerChain sub)
        {
            this.messageHandlerChain = sub;
        }
    }
}