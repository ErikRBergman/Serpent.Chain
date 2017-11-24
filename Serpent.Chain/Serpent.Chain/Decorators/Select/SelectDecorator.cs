namespace Serpent.Chain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    internal class SelectDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly ChainBuilder<TNewMessageType> newchainBuilder;

        private readonly Func<TOldMessageType, TNewMessageType> selector;

        private IChain<TNewMessageType> newChain;

        public SelectDecorator(ChainBuilder<TNewMessageType> newchainBuilder, Func<TOldMessageType, TNewMessageType> selector)
        {
            this.newchainBuilder = newchainBuilder;
            this.selector = selector;
        }

        public Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            return this.newChain.HandleMessageAsync(this.selector(message), cancellationToken);
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