namespace Serpent.Common.MessageBus.BusPublishers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class BusPublisher<T>
    {
        public abstract Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message);
    }
}
