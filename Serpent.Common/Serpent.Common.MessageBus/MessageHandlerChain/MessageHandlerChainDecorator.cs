namespace Serpent.Common.MessageBus.MessageHandlerChain
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The base type for class based message handler chain decorators
    /// </summary>
    /// <typeparam name="TMessageType"></typeparam>
    public abstract class MessageHandlerChainDecorator<TMessageType>
    {
        /// <summary>
        /// Handle a message
        /// </summary>
        /// <param name="message">
        /// Thge message
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public abstract Task HandleMessageAsync(TMessageType message, CancellationToken cancellationToken);
    }
}