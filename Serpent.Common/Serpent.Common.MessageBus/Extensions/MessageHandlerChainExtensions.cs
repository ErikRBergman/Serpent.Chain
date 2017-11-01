// ReSharper disable CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides message handler chain extensions
    /// </summary>
    public static class MessageHandlerChainExtensions
    {
        /// <summary>
        /// Handle a message on the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChain">The message handler chain</param>
        /// <param name="message">The message to handle</param>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns>A task that completes when the message is handled</returns>
        public static Task HandleMessageAsync<TMessageType>(this IMessageHandlerChain<TMessageType> messageHandlerChain, TMessageType message, CancellationToken cancellationToken)
        {
            return messageHandlerChain.MessageHandlerChainFunc(message, cancellationToken);
        }

        /// <summary>
        /// Handle a message on the message handler chain
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChain">The message handler chain</param>
        /// <param name="message">The message to handle</param>
        /// <returns>A task that completes when the message is handled</returns>
        public static Task HandleMessageAsync<TMessageType>(this IMessageHandlerChain<TMessageType> messageHandlerChain, TMessageType message)
        {
            return messageHandlerChain.MessageHandlerChainFunc(message, CancellationToken.None);
        }

    }
}