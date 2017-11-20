// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using Serpent.MessageHandlerChain.Decorators.Skip;

    /// <summary>
    ///     Provides the .Skip() decorator extensions
    /// </summary>
    public static class SkipExtensions
    {
        /// <summary>
        ///     Skips a fixed number of messages before allowing all messages to pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="numberOfMessages">The number of messages to skip</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Skip<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, int numberOfMessages)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new SkipDecorator<TMessageType>(currentHandler, numberOfMessages));
        }
    }
}