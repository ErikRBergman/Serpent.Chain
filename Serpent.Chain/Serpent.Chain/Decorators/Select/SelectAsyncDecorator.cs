namespace Serpent.Chain.Decorators.Select
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    internal class SelectAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly ChainBuilder<TNewMessageType> newchainBuilder;

        private readonly Func<TOldMessageType, Task<TNewMessageType>> selector;

        private IChain<TNewMessageType> newChain;

        public SelectAsyncDecorator(ChainBuilder<TNewMessageType> newchainBuilder, Func<TOldMessageType, Task<TNewMessageType>> selector)
        {
            this.newchainBuilder = newchainBuilder;
            this.selector = selector;
        }

        public async Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            await this.newChain.HandleMessageAsync(await this.selector(message), cancellationToken);
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