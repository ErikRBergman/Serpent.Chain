// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.Interfaces;

    /// <summary>
    /// Base type for bus publishers
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public abstract class BusPublisher<TMessageType>
    {
        /// <summary>
        /// Publish a message to all subscriptions
        /// </summary>
        /// <param name="subscriptions">The subscriptions</param>
        /// <param name="message">The message to publish</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>The task</returns>
        public abstract Task PublishAsync(IEnumerable<IMessageHandler<TMessageType>> subscriptions, TMessageType message, CancellationToken cancellationToken);
    }
}