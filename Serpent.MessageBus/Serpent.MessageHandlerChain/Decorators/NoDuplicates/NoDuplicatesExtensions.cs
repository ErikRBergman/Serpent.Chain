// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.Decorators.NoDuplicates;
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    ///     The no duplicates decorator extensions type
    /// </summary>
    public static class NoDuplicatesExtensions
    {
        /// <summary>
        ///     Prevents more than once identical message from being handled concurrently. Messages not handled are dropped.
        ///     If the message type is a value type, it's value is used.
        ///     If the message type is a reference type, that instance of the message can not be handled concurrently
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> NoDuplicates<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new NoDuplicatesDecorator<TMessageType, TMessageType>(currentHandler, m => m));
        }

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
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new NoDuplicatesDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        ///     Prevents more than a single message with the same key from being handled concurrently.
        ///     The keySelector defines the key. All messages identified as duplicates are dropped.
        /// </summary>
        /// <typeparam name="TMessageType">
        ///     The message type
        /// </typeparam>
        /// <param name="messageHandlerChainBuilder">
        ///     The message handler chain builder
        /// </param>
        /// <param name="config">An action that configures the no duplicates decorator</param>
        /// <returns>
        ///     The message handler chain builder
        /// </returns>
        public static IMessageHandlerChainBuilder<TMessageType> NoDuplicates<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<NoDuplicatesDecoratorBuilder<TMessageType>> config)
        {
            var builder = new NoDuplicatesDecoratorBuilder<TMessageType>();
            config(builder);
            return messageHandlerChainBuilder.AddDecorator(builder);
        }
    }
}