// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    /// <summary>
    /// The cast decorator extensions
    /// </summary>
    public static class CastExtensions
    {
        /// <summary>
        /// Cast the message to a new type, returning a message handler chain builder of the new type
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TNewType">The new message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain of the new type</returns>
        public static IMessageHandlerChainBuilder<TNewType> Cast<TMessageType, TNewType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
            where TNewType : class
        {
            return messageHandlerChainBuilder.Select(m => m as TNewType);
        }
    }
}