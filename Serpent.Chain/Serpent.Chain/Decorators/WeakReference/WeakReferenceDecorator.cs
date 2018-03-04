// ReSharper disable StyleCop.SA1126 - invalid warning
namespace Serpent.Chain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;

    internal class WeakReferenceDecorator<TMessageType> : ChainDecorator<TMessageType>, IWeakReferenceGarbageCollection
    {
        private readonly WeakReference<IMessageHandler<TMessageType>> weakReferenceMessageHandler;

        private IChain chain;

        public WeakReferenceDecorator(
            IMessageHandler<TMessageType> handler,
            IChainBuilderNotifier builderNotifier,
            IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            this.weakReferenceMessageHandler = new WeakReference<IMessageHandler<TMessageType>>(handler);
            builderNotifier.AddNotification(chain => this.chain = chain);
            weakReferenceGarbageCollector?.Add(this);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.weakReferenceMessageHandler.TryGetTarget(out var messageHandler))
            {
                return messageHandler.HandleMessageAsync(message, token);
            }

            this.chain?.Dispose();
            this.chain = null;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool DisposeIfReclamiedByGarbageCollection()
        {
            if (this.weakReferenceMessageHandler.TryGetTarget(out _))
            {
                return false;
            }

            this.chain?.Dispose();
            this.chain = null;

            return true;
        }
    }
}