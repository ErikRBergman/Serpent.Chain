// ReSharper disable once CheckNamespace

namespace Serpent.Common.MessageBus
{
    using System;
    using System.Collections.Generic;

    using Serpent.Common.MessageBus.MessageHandlerChain.Decorators.NoDuplicates;
    using Serpent.Common.MessageBus.MessageHandlerChain.WireUp;

    /// <summary>
    ///     The no duplicates decorator extensions type
    /// </summary>
    public static class NoDuplicatesExtensions
    {
        /// <summary>
        ///     Prevents more than a single message with the same key from being handled concurrently.
        ///     The keySelector defines the key. All messages identified as duplicates are dropped.
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="keySelector">The key selector used to detemine the key</param>
        /// <returns>The message handler chain builder</returns>
        [ExtensionMethodSelector(NoDuplicatesWireUp.WireUpExtensionName)]
        public static IMessageHandlerChainBuilder<TMessageType> NoDuplicates<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new NoDuplicatesDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        ///     Prevents more than a single message with the same key from being handled concurrently.
        ///     The keySelector defines the key. All messages identified as duplicates are dropped.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <typeparam name="TKeyType">
        ///     The key type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="keySelector">
        ///     The key selector used to detemine the key
        /// </param>
        /// <param name="equalityComparer">
        ///     The equality Comparer
        /// </param>
        /// <returns>
        ///     The message handler chain builder
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> NoDuplicates<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector,
            IEqualityComparer<TKeyType> equalityComparer)
        {
            return messageHandlerChainBuilder.Add(currentHandler => new NoDuplicatesDecorator<TMessageType, TKeyType>(currentHandler, keySelector, equalityComparer));
        }
    }
}