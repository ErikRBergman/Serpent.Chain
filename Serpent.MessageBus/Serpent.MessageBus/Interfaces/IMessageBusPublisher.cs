// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message bus publisher interface. Used to both publish messages to a message bus.
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageBusPublisher<in TMessageType>
    {
        /// <summary>
        /// Publishes a message
        /// </summary>
        /// <param name="message">
        /// The message
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task PublishAsync(TMessageType message, CancellationToken cancellationToken);
    }
}