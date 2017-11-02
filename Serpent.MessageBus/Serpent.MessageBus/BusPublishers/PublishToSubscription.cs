namespace Serpent.MessageBus.BusPublishers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.MessageBus.Models;

    /// <summary>
    /// The publish to subscription type
    /// </summary>
    public static class PublishToSubscription
    {
        /// <summary>
        /// Publish a message to a message handler
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageAndHandler">The message type</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task</returns>
        public static Task PublishAsync<TMessageType>(MessageAndHandler<TMessageType> messageAndHandler, CancellationToken cancellationToken)
        {
            return messageAndHandler.MessageHandler(messageAndHandler.Message, cancellationToken);
        }
    }
}