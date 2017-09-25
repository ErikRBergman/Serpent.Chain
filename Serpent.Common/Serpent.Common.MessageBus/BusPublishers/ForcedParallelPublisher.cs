// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ForcedParallelPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new ForcedParallelPublisher<T>();

        public override async Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message)
        {
            foreach (var subscription in subscriptions)
            {
                var subscriptionHandlerFunc = subscription.SubscriptionHandlerFunc;
                if (subscriptionHandlerFunc != null)
                {
                    await Task.Run(() => subscriptionHandlerFunc(message)).ConfigureAwait(false);
                }
            }
        }
    }
}