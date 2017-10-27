// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;
    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.SkipWhile;

    /// <summary>
    /// Provides extensions for the .SkipWhile decorator
    /// </summary>
    public static class SkipWhileExtensions
    {
        /// <summary>
        /// Skips messages while the predicate returns true, then passes all through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate to determine whether to skip initial messages</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SkipWhileDecorator<TMessageType>(currentHandler, predicate));
        }

        /// <summary>
        /// Skips messages while the predicate returns true, then passes all through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate to determine whether to skip initial messages</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> SkipWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new SkipWhileAsyncDecorator<TMessageType>(currentHandler, predicate));
        }
    }
}