// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;
    using System.Threading.Tasks;

    using Serpent.MessageHandlerChain.Decorators.Distinct;
    using Serpent.MessageHandlerChain.WireUp;

    /// <summary>
    ///     The distinct decorator extensions
    /// </summary>
    public static class DistinctExtensions
    {
        /// <summary>
        ///     Lets a message with the specified key through once
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The key selector</param>
        /// <returns>The builder</returns>
        [ExtensionMethodSelector(DistinctWireUp.WireUpExtensionName)]
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, TKeyType> keySelector)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new DistinctDecorator<TMessageType, TKeyType>(currentHandler, keySelector));
        }

        /// <summary>
        ///     Lets a message pass through once.
        ///     If the message is a value type (like int, or float) the value is for key.
        ///     If the message is a reference type (like a class or interface), the reference is used for key
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new DistinctDecorator<TMessageType, TMessageType>(currentHandler, m => m));
        }

        /// <summary>
        ///     Lets a message with the specified key pass through once
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="config">The action used to configure the distinct decorator</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<DistinctDecoratorBuilder<TMessageType>> config)
        {
            var builder = new DistinctDecoratorBuilder<TMessageType>();
            config(builder);
            return messageHandlerChainBuilder.AddDecorator(builder);
        }

        /// <summary>
        ///     Let only a single message with the specified key pass through
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <typeparam name="TKeyType">The key type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="keySelector">The ASYNC key selector</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Distinct<TMessageType, TKeyType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Func<TMessageType, Task<TKeyType>> keySelector)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new DistinctAsyncDecorator<TMessageType, TKeyType>(currentHandler, (message, token) => keySelector(message)));
        }
    }
}