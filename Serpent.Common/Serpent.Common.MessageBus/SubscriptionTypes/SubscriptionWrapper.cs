//// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    public class SubscriptionWrapper<T> : IDisposable, IMessageBusSubscription
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

        public static SubscriptionWrapper<T> Create(IMessageBusSubscriber<T> messageBus, Func<T, Task> invocationFunc, Func<T, bool> messageFilterFunc = null)
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

            return new SubscriptionWrapper<T>(subscription);
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