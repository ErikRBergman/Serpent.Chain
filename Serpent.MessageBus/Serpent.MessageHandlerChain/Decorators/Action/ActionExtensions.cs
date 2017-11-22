// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.Decorators.Action;

    /// <summary>
    ///     The filter extensions
    /// </summary>
    public static class ActionExtensions
    {
        /// <summary>
        ///     Adds an action decorator
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The message handler chain builder</param>
        /// <param name="configure">Sets up the events to handle</param>
        /// <returns>The message handler chain builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Action<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            Action<IActionDecoratorBuilder<TMessageType>> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new ActionDecoratorBuilder<TMessageType>();
            configure(builder);
            return messageHandlerChainBuilder.AddDecorator(builder);
        }
    }
}