namespace Serpent.MessageHandlerChain.Decorators.SelectMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Interfaces;
    using Serpent.MessageHandlerChain.Notification;

    internal class SelectManyDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly MessageHandlerChainBuilder<TNewMessageType> newChainBuilder;

        private readonly Func<TOldMessageType, IEnumerable<TNewMessageType>> selector;

        private IMessageHandlerChain<TNewMessageType> newChain;

        public SelectManyDecorator(MessageHandlerChainBuilder<TNewMessageType> newChainBuilder, Func<TOldMessageType, IEnumerable<TNewMessageType>> selector)
        {
            this.newChainBuilder = newChainBuilder;
            this.selector = selector;
        }

        public Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            return Task.WhenAll(this.selector(message).Select(msg => this.newChain.HandleMessageAsync(msg, cancellationToken)));
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