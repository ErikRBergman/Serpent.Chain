// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    public class SerialPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        public static BusPublisher<TMessageType> Default { get; } = new SerialPublisher<TMessageType>();

        public override async Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> subscriptions, TMessageType message, CancellationToken cancellationToken)
        {
            foreach (var subscription in subscriptions)
            {
                await subscription(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}