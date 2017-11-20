namespace Serpent.MessageHandlerChain.Interfaces
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
        ///  Handles a message
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A task that completes when the message is handled</returns>
        Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}