// ReSharper disable once CheckNamespace
namespace Serpent.MessageHandlerChain
{
    using Serpent.MessageHandlerChain.Decorators.SoftFireAndForget;

    /// <summary>
    /// Provides extensions for the .SoftFireAndForget decorator
    /// </summary>
    public static class SoftFireAndForgetExtensions
    {
        /// <summary>
        /// Drops the the feedback chain, making the message handler chain fire and forget unless the message handler chain returns synchronously
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> SoftFireAndForget<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new SoftFireAndForgetDecorator<TMessageType>(currentHandler).HandleMessageAsync);
        }
    }
}