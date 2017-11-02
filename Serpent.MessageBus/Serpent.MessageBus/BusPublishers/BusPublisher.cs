// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base type for bus publishers
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public abstract class BusPublisher<TMessageType>
    {
        /// <summary>
        /// Publishes a message to all subscribers (message handlers). This method is called when a message is published to a message bus.
        /// </summary>
        /// <param name="handlers">The message handlers</param>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The task</returns>
        public abstract Task PublishAsync(IEnumerable<Func<TMessageType, CancellationToken, Task>> handlers, TMessageType message, CancellationToken cancellationToken);
    }
}