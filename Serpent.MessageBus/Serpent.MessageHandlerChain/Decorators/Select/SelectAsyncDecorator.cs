namespace Serpent.MessageHandlerChain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;
    using Serpent.MessageHandlerChain.Notification;

    internal class SelectAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly MessageHandlerChainBuilder<TNewMessageType> newChainBuilder;

        private readonly Func<TOldMessageType, Task<TNewMessageType>> selector;

        private IMessageHandlerChain<TNewMessageType> newChain;

        public SelectAsyncDecorator(MessageHandlerChainBuilder<TNewMessageType> newChainBuilder, Func<TOldMessageType, Task<TNewMessageType>> selector)
        {
            this.newChainBuilder = newChainBuilder;
            this.selector = selector;
        }

        public async Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            await this.newChain.HandleMessageAsync(await this.selector(message), cancellationToken);
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