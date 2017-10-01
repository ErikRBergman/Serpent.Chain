// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ForcedParallelPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new ForcedParallelPublisher<T>();

        public override Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message, CancellationToken token)
        {
            foreach (var subscription in subscriptions)
            {
                var subscriptionHandlerFunc = subscription.SubscriptionHandlerFunc;
                if (subscriptionHandlerFunc != null)
                {
                    Task.Run(() => subscriptionHandlerFunc(message, token), token);
                }
            }

            return Task.CompletedTask;
        }
    }
}