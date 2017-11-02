// ReSharper disable StyleCop.SA1126 - invalid warning
namespace Serpent.MessageBus.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Interfaces;

    internal class WeakReferenceDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IWeakReferenceGarbageCollection
    {
        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> handlerFunc;

        private IMessageHandlerChain messageHandlerChain;

        public WeakReferenceDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IMessageHandlerChainBuildNotification buildNotification, IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            this.handlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(handlerFunc);
            buildNotification.AddNotification(chain => this.messageHandlerChain = chain);
            weakReferenceGarbageCollector?.Add(this);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.handlerFunc.TryGetTarget(out var target))
            {
                return target(message, token);
            }

            if (this.messageHandlerChain != null)
            {
                this.messageHandlerChain.Dispose();
                this.messageHandlerChain = null;
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool DisposeSubscriptionIfReclamiedByGarbageCollection()
        {
            if (this.handlerFunc.TryGetTarget(out var _))
            {
                return false;
            }

            if (this.messageHandlerChain != null)
            {
                this.messageHandlerChain.Dispose();
                this.messageHandlerChain = null;
            }

            return true;
        }
    }
}