namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    internal struct WeakReferenceSubscription<TMessageType> : ISubscription<TMessageType>
    {
        private readonly WeakReference<Func<TMessageType, CancellationToken, Task>> subscriptionHandlerFunc;

        public WeakReferenceSubscription(Func<TMessageType, CancellationToken, Task> subscriptionHandlerFunc)
        {
            this.subscriptionHandlerFunc = new WeakReference<Func<TMessageType, CancellationToken, Task>>(subscriptionHandlerFunc);
        }

        public Func<TMessageType, CancellationToken, Task> SubscriptionHandlerFunc
        {
            get
            {
                if (this.subscriptionHandlerFunc.TryGetTarget(out var target))
                {
                    return target;
                }

                return null;
            }
        }
    }
}