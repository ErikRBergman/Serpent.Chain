namespace Serpent.Chain.Decorators.SelectMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    internal class SelectManyDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly ChainBuilder<TNewMessageType> newchainBuilder;

        private readonly Func<TOldMessageType, IEnumerable<TNewMessageType>> selector;

        private IChain<TNewMessageType> newChain;

        public SelectManyDecorator(ChainBuilder<TNewMessageType> newchainBuilder, Func<TOldMessageType, IEnumerable<TNewMessageType>> selector)
        {
            this.newchainBuilder = newchainBuilder;
            this.selector = selector;
        }

        public Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            return Task.WhenAll(this.selector(message).Select(msg => this.newChain.HandleMessageAsync(msg, cancellationToken)));
        }

        public void ChainBuilt(IChain chain)
        {
            var subscriptionNotification = new ChainBuilderNotifier();
            var services = new ChainBuilderSetupServices(subscriptionNotification);
            var chainFunc = this.newchainBuilder.BuildFunc(services);
            this.newChain = new Chain<TNewMessageType>(chainFunc, chain.Dispose);
            subscriptionNotification.Notify(this.newChain);
        }
    }
}