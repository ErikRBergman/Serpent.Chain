// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SerialPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new SerialPublisher<T>();

        public override async Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message)
        {
            foreach (var subscription in subscriptions)
            {
                var subscriptionHandlerFunc = subscription.SubscriptionHandlerFunc;
                if (subscriptionHandlerFunc != null)
                {
                    await subscriptionHandlerFunc(message);
                }
            }
        }
    }
}