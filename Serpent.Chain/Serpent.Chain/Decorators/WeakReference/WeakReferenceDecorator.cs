// ReSharper disable StyleCop.SA1126 - invalid warning
namespace Serpent.Chain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Chain.Interfaces;

    internal class WeakReferenceDecorator<TMessageType> : ChainDecorator<TMessageType>, IWeakReferenceGarbageCollection
    {
        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> handlerFunc;

        private IChain chain;

        public WeakReferenceDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IChainBuilderNotifier builderNotifier, IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            this.handlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(handlerFunc);
            builderNotifier.AddNotification(chain => this.chain = chain);
            weakReferenceGarbageCollector?.Add(this);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.handlerFunc.TryGetTarget(out var target))
            {
                return target(message, token);
            }

            this.chain?.Dispose();
            this.chain = null;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool DisposeIfReclamiedByGarbageCollection()
        {
            if (this.handlerFunc.TryGetTarget(out var _))
            {
                return false;
            }

            this.chain?.Dispose();
            this.chain = null;

            return true;
        }
    }
}