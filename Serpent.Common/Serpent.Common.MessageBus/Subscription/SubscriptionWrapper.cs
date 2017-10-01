//// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SubscriptionWrapper : IMessageBusSubscription
    {
        private IMessageBusSubscription subscription;

        public SubscriptionWrapper(IMessageBusSubscription subscription)
        {
            this.subscription = subscription;
        }

        ~SubscriptionWrapper()
        {
            this.Dispose();
        }

        public static SubscriptionWrapper Create(IMessageBusSubscription messageBusSubscription)
        {
            return new SubscriptionWrapper(messageBusSubscription);
        }

        public static SubscriptionWrapper Create<TMessageType>(IMessageBusSubscriptions<TMessageType> messageBus, Func<TMessageType, CancellationToken, Task> invocationFunc, Func<TMessageType, bool> messageFilterFunc = null)
        {
            IMessageBusSubscription subscription;

            if (messageFilterFunc != null)
            {
                subscription = messageBus.Subscribe((message, token) => messageFilterFunc(message) ? invocationFunc(message, token) : Task.CompletedTask);
            }
            else
            {
                subscription = messageBus.Subscribe(invocationFunc);
            }

            return new SubscriptionWrapper(subscription);
        }

        public void Unsubscribe()
        {
            this.subscription?.Dispose();
            this.subscription = null;
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}