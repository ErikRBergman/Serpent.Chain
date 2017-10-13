// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
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
        /// Used to publish messages to the message bus
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>A task</returns>
        Task PublishAsync(TMessageType message, CancellationToken cancellationToken);
    }
}