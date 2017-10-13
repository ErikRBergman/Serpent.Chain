namespace Serpent.Common.MessageBus.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The message handler interface
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public interface IMessageHandler<in TMessageType>
    {
        /// <summary>
        /// The method invoked when a message is published
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A task</returns>
        Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}