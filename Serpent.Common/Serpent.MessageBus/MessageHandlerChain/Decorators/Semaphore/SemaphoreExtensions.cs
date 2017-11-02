// ReSharper disable once CheckNamespace

namespace Serpent.MessageBus
{
    using System;
    using System.Collections.Generic;

    using Serpent.MessageBus.MessageHandlerChain.Decorators.Semaphore;

    /// <summary>
    ///     The semaphore MHCBuilder extensions
    /// </summary>
    public static class SemapshoreExtensions
    {
        /// <summary>
        ///     Limits the message handler chain to X concurrent messages being handled.
        ///     This method does not add concurrency but limits it.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The type of the messages type in the message handler chain
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="maxNumberOfConcurrentMessages">
        ///     The maximum number of concurrent messages being handled
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int maxNumberOfConcurrentMessages)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new SemaphoreDecorator<TMessageType>(currentHandler, maxNumberOfConcurrentMessages));
        }

        /// <summary>
        ///     Limits the message handler chain to X concurrent messages per unique key being handled.
        ///     This method does not add concurrency but limits it.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The type of the messages type in the message handler chain
        /// </typeparam>
        /// <typeparam name="TKeyType">The type of the key used to check for concurrency</typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="keySelector">
        ///     The key Selector.
        /// </param>
        /// <param name="maxNumberOfConcurrentMessages">
        ///     The maximum number of concurrent messages being handled
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector,
            int maxNumberOfConcurrentMessages = 1)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new SemaphoreWithKeyDecorator<TMessageType, TKeyType>(currentHandler, keySelector, maxNumberOfConcurrentMessages));
        }

        /// <summary>
        ///     Limits the message handler chain to X concurrent messages per unique key being handled.
        ///     This method does not add concurrency but limits it.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The type of the messages type in the message handler chain
        /// </typeparam>
        /// <typeparam name="TKeyType">
        ///     The type of the key used to check for concurrency
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="keySelector">
        ///     The key Selector.
        /// </param>
        /// <param name="equalityComparer">
        ///     The equality Comparer.
        /// </param>
        /// <param name="maxNumberOfConcurrentMessages">
        ///     The maximum number of concurrent messages being handled
        /// </param>
        /// <returns>
        ///     The <see cref="IMessageHandlerChainBuilder&lt;TMessageType&gt;" />.
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector,
            IEqualityComparer<TKeyType> equalityComparer,
            int maxNumberOfConcurrentMessages = 1)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new SemaphoreWithKeyDecorator<TMessageType, TKeyType>(
                    currentHandler,
                    keySelector,
                    new KeySemaphore<TKeyType>(maxNumberOfConcurrentMessages, equalityComparer)));
        }

        /// <summary>
        ///     Limit concurrency of messages by key, using a shared KeySemaphore
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="keySemaphore">The shared key semaphore</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Semaphore<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector,
            KeySemaphore<TKeyType> keySemaphore)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new SemaphoreWithKeyDecorator<TMessageType, TKeyType>(currentHandler, keySelector, keySemaphore));
        }
    }
}