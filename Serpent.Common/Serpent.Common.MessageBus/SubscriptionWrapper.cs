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

        public static SubscriptionWrapper<T> Create(IMessageBus<T> messageBus, Func<T, Task> invocationFunc, Func<T, bool> eventFilterFunc = null)
        {
            IMessageBusSubscription subscription;

            if (eventFilterFunc != null)
            {
                subscription = messageBus.Subscribe(arg => eventFilterFunc(arg) ? invocationFunc(arg) : Task.CompletedTask);
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