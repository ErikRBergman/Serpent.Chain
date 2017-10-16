namespace Serpent.Common.MessageBus.Extras
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A bridge to allow registering IMessageBusPublisher to the same IMessageBus with simpler IOC containers, like the one in ASP NET Core
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public struct PublisherBridge<TMessageType> : IMessageBusPublisher<TMessageType>
    {
        private readonly IMessageBus<TMessageType> messageBus;

        /// <summary>
        /// Creates an instance of the publisher bridge
        /// </summary>
        /// <param name="messageBus">The message bus</param>
        public PublisherBridge(IMessageBus<TMessageType> messageBus)
        {
            this.messageBus = messageBus;
        }

        /// <summary>
        /// Publishes a message
        /// </summary>
        /// <param name="message">
        /// The message
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public Task PublishAsync(TMessageType message, CancellationToken cancellationToken)
        {
            return this.messageBus.PublishAsync(message, cancellationToken);
        }
    }
}