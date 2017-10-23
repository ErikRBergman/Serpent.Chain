// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.First;

    /// <summary>
    /// The first decorator extensions
    /// </summary>
    public static class FirstExtensions
    {
        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="predicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, bool> predicate)
        {
            return messageHandlerChainBuilder.Add((currentHandler, subscriptionServices) => new FirstDecorator<TMessageType>(currentHandler, predicate, subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.Add((currentHandler, subscriptionServices) => new FirstDecorator<TMessageType>(currentHandler, msg => true, subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="asyncPredicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<bool>> asyncPredicate)
        {
            return messageHandlerChainBuilder.Add((currentHandler, subscriptionServices) => new FirstAsyncDecorator<TMessageType>(currentHandler, (message, token) => asyncPredicate(message), subscriptionServices).HandleMessageAsync);
        }

        /// <summary>
        /// Handle only the first message where the predicate returns true, then unsubscribe
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="asyncPredicate">The predicate</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> First<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, CancellationToken, Task<bool>> asyncPredicate)
        {
            return messageHandlerChainBuilder.Add((currentHandler, subscriptionServices) => new FirstAsyncDecorator<TMessageType>(currentHandler, asyncPredicate, subscriptionServices).HandleMessageAsync);
        }
    }
}