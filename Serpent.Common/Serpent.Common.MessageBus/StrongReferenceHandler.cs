namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    internal struct StrongReferenceHandler<TMessageType> : IMessageHandler<TMessageType>
    {
        public StrongReferenceHandler(Func<TMessageType, CancellationToken, Task> subscriptionHandlerFunc)
        {
            this.SubscriptionHandlerFunc = subscriptionHandlerFunc;
        }

        public Func<TMessageType, CancellationToken, Task> SubscriptionHandlerFunc { get; }

        public Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken)
        {
            return this.SubscriptionHandlerFunc(message, cancellationToken);
        }
    }
}