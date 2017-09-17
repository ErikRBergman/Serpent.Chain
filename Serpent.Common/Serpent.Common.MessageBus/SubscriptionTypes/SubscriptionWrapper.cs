//// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public class SubscriptionWrapper: IDisposable, IMessageBusSubscription
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

        public static SubscriptionWrapper Create<T>(IMessageBusSubscriber<T> messageBus, Func<T, Task> invocationFunc, Func<T, bool> messageFilterFunc = null)
        {
            IMessageBusSubscription subscription;

            if (messageFilterFunc != null)
            {
                subscription = messageBus.Subscribe(arg => messageFilterFunc(arg) ? invocationFunc(arg) : Task.CompletedTask);
            }
            else
            {
                subscription = messageBus.Subscribe(invocationFunc);
            }

            return new SubscriptionWrapper(subscription);
        }

        public void Unsubscribe()
        {
            this.subscription?.Unsubscribe();
            this.subscription = null;
        }

        public void Dispose()
        {
            this.Unsubscribe();
        }
    }
}