// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.TakeWhile;

    /// <summary>
    /// Provides extension for the .TakeWhile() decorator
    /// </summary>
    public static class TakeWhileExtensions
    {
        /// <summary>
        /// Handle messages as long as the predicate returns true, then unsubscribes
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate used to determine if messages can continue to flow</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TakeWhileDecorator<TMessageType>(currentHandler, predicate));
        }

        /// <summary>
        /// Handle messages as long as the predicate returns true, then unsubscribes
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate used to determine if messages can continue to flow</param>
        /// <returns>A message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> TakeWhile<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> predicate)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new TakeWhileAsyncDecorator<TMessageType>(currentHandler, predicate));
        }
    }
}