// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ParallelPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new ParallelPublisher<T>();

        public override Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message)
        {
            return Task.WhenAll(
                subscriptions.Select(
                    s =>
                        {
                            var subscriptionHandlerFunc = s.SubscriptionHandlerFunc;
                            return subscriptionHandlerFunc == null ? Task.CompletedTask : subscriptionHandlerFunc(message);
                        }));
        }
    }
}