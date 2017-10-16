namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal struct StrongReferenceSubscription<TMessageType> : ISubscription<TMessageType>
    {
        public StrongReferenceSubscription(Func<TMessageType, CancellationToken, Task> subscriptionHandlerFunc)
        {
            this.SubscriptionHandlerFunc = subscriptionHandlerFunc;
        }

        public Func<TMessageType, CancellationToken, Task> SubscriptionHandlerFunc { get; }
    }
}