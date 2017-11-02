// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    /// <summary>
    /// The .OfType() decorator extensions
    /// </summary>
    public static class OfTypeExtensions
    {
        /// <summary>
        /// Filters the message flow to only forward messages of the TNewType, casting those messages to TNewType
        /// </summary>
        /// <typeparam name="TMessageType">The current message type</typeparam>
        /// <typeparam name="TNewType">The type to cast the message type to</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TNewType> OfType<TMessageType, TNewType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
            where TNewType : class, TMessageType
        {
            return messageHandlerChainBuilder.Where(m => m is TNewType).Select(m => m as TNewType);
        }
    }
}