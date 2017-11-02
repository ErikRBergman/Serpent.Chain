// ReSharper disable once CheckNamespace
namespace Serpent.MessageBus
{
    using Serpent.MessageBus.MessageHandlerChain.Decorators.Take;

    /// <summary>
    /// Provides the .Take() decorator extensions for message handler chain builder 
    /// </summary>
    public static class TakeExtensions
    {
        /// <summary>
        /// Only takes a specified number of messages before unsubscribing
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="numberOfMessages">The number of messages to take</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Take<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int numberOfMessages)
        {
            return messageHandlerChainBuilder.AddDecorator((currentHandler, services) => new TakeDecorator<TMessageType>(currentHandler, numberOfMessages, services.BuildNotification));
        }
    }
}