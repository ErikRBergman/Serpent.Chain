namespace Serpent.MessageHandlerChain
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The base type for class based message handler chain decorators
    /// </summary>
    /// <typeparam name="TMessageType">The message type</typeparam>
    public abstract class MessageHandlerChainDecorator<TMessageType>
    {
        /// <summary>
        /// Handles a message.
        /// </summary>
        /// <param name="message">
        /// The message to handle.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that should be used to cancel the message handler.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that completes when the message is handled.
        /// </returns>
        public abstract Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}