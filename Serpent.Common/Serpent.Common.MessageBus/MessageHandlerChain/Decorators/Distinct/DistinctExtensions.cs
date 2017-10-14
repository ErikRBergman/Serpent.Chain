// ReSharper disable once CheckNamespace
namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.Distinct;
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    public static class DistinctExtensions
    {
        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The key selector</param>
        /// <returns>The builder</returns>
        [ExtensionMethodSelector(DistinctWireUp.WireUpExtensionName)]
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, TKeyType> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The key selector</param>
        /// <param name="equalityComparer">The equality comparer</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, TKeyType> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }

        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task<TKeyType>> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message)));
        }

        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <param name="equalityComparer">The equality comparer</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message), equalityComparer));
        }

        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        /// Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <param name="equalityComparer">The equality comparer</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, Func<TMessageType, CancellationToken, Task<TKeyType>> keySelector, IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }
    }
}