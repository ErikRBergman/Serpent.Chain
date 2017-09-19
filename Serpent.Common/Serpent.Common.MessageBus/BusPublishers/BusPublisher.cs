// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class BusPublisher<T>
    {
        public abstract Task PublishAsync(IEnumerable<ISubscription<T>> subscriptions, T message);
    }
}