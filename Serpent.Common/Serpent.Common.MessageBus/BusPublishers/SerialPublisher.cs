// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The serial publisher type
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public class SerialPublisher<TMessageType> : BusPublisher<TMessageType>
    {
        /// <summary>
        /// A default serial publisher
        /// </summary>
        public static BusPublisher<TMessageType> Default { get; } = new SerialPublisher<TMessageType>();

        /// <summary>
        /// Publish a message to all handlers. This method is called when a message is published to a message bus.
        /// </summary>
        /// <param name="handlers">The handlers</param>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The task</returns>
        public override async Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken cancellationToken)
        {
            foreach (var handler in handlers)
            {
                await handler(message, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}