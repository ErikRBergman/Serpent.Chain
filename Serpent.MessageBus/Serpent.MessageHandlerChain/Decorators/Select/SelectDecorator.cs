namespace Serpent.MessageHandlerChain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;
    using Serpent.MessageHandlerChain.Notification;

    internal class SelectDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly MessageHandlerChainBuilder<TNewMessageType> newChainBuilder;

        private readonly Func<TOldMessageType, TNewMessageType> selector;

        private IMessageHandlerChain<TNewMessageType> newChain;

        public SelectDecorator(MessageHandlerChainBuilder<TNewMessageType> newChainBuilder, Func<TOldMessageType, TNewMessageType> selector)
        {
            this.newChainBuilder = newChainBuilder;
            this.selector = selector;
        }

        public Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            return this.newChain.HandleMessageAsync(this.selector(message), cancellationToken);
        }

        public void MessageHandlerChainBuilt(IMessageHandlerChain messageHandlerChain)
        {
            var subscriptionNotification = new MessageHandlerChainBuildNotification();
            var services = new MessageHandlerChainBuilderSetupServices(subscriptionNotification);
            var chainFunc = this.newChainBuilder.BuildFunc(services);
            this.newChain = new MessageHandlerChain<TNewMessageType>(chainFunc, messageHandlerChain.Dispose);
            subscriptionNotification.Notify(this.newChain);
        }
    }
}