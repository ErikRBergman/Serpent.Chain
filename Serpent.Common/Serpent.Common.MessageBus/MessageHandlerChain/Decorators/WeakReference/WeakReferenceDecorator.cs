namespace Serpent.Common.MessageBus.MessageHandlerChain.Decorators.WeakReference
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    internal class WeakReferenceDecorator<TMessageType> : MessageHandlerChainDecorator<TMessageType>, IWeakReferenceGarbageCollection
    {
        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> handlerFunc;

        private IMessageBusSubscription subscription;

        public WeakReferenceDecorator(Func<TMessageType, CancellationToken, Task> handlerFunc, IMessageHandlerChainSubscriptionNotification subscriptionNotification, IWeakReferenceGarbageCollector weakReferenceGarbageCollector)
        {
            this.handlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(handlerFunc);
            subscriptionNotification.AddNotification(sub => this.subscription = sub);
            weakReferenceGarbageCollector?.Add(this);
        }

        public override Task HandleMessageAsync(TMessageType message, CancellationToken token)
        {
            if (this.handlerFunc.TryGetTarget(out var target))
            {
                return target(message, token);
            }

            if (this.subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
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

            if (this.subscription != null)
            {
                this.subscription.Dispose();
                this.subscription = null;
            }

            return true;
        }
    }
}