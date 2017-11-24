namespace Serpent.Chain.Decorators.SelectMany
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;
    using Serpent.Chain.Notification;

    internal class SelectManyAsyncDecorator<TOldMessageType, TNewMessageType> : IMessageHandler<TOldMessageType>
    {
        private readonly ChainBuilder<TNewMessageType> newchainBuilder;

        private readonly Func<TOldMessageType, Task<IEnumerable<TNewMessageType>>> selector;

        private IChain<TNewMessageType> newChain;

        public SelectManyAsyncDecorator(ChainBuilder<TNewMessageType> newchainBuilder, Func<TOldMessageType, Task<IEnumerable<TNewMessageType>>> selector)
        {
            this.newchainBuilder = newchainBuilder;
            this.selector = selector;
        }

        public async Task HandleMessageAsync(TOldMessageType message, CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                (await this.selector(message).ConfigureAwait(false))
                    .Select(msg => this.newChain.HandleMessageAsync(msg, cancellationToken))).ConfigureAwait(false);
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