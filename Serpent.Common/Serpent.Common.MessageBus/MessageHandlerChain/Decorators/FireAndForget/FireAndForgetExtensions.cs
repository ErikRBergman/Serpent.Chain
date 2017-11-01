// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.FireAndForget;

    /// <summary>
    /// The fire and forget decorator extensions
    /// </summary>
    public static class FireAndForgetExtensions
    {
        /// <summary>
        /// Execute each message handled on a new Task
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> FireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new FireAndForgetDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}