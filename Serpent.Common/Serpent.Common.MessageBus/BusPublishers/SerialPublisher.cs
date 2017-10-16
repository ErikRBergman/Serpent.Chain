// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class SerialPublisher<T> : BusPublisher<T>
    {
        public static BusPublisher<T> Default { get; } = new SerialPublisher<T>();

        public override async Task PublishAsync(IEnumerable<IMessageHandler<T>> subscriptions, T message, CancellationToken cancellationToken)
        {
            foreach (var subscription in subscriptions)
            {
                await subscription.HandleMessageAsync(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}