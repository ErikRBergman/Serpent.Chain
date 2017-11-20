// ReSharper disable once CheckNamespace

namespace Serpent.MessageHandlerChain
{
    using System;

    using Serpent.MessageHandlerChain.Decorators.Delay;

    /// <summary>
    /// The delay decorator extensions
    /// </summary>
    public static class DelayExtensions
    {
        /// <summary>
        /// Delay handling each message by a specified time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="timeToWait">The timespan to await</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Delay<TMessageType>(this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder, TimeSpan timeToWait)
        {
            return messageHandlerChainBuilder.AddDecorator(currentHandler => new DelayDecorator<TMessageType>(currentHandler, timeToWait).HandleMessageAsync);
        }

        /// <summary>
        /// Delay handling each message by a specified time
        /// </summary>
        /// <typeparam name="TMessageType">The message type</typeparam>
        /// <param name="messageHandlerChainBuilder">The builder</param>
        /// <param name="timeInMilliseconds">The time to await in milliseconds</param>
        /// <returns>The builder</returns>
        public static IMessageHandlerChainBuilder<TMessageType> Delay<TMessageType>(
            this IMessageHandlerChainBuilder<TMessageType> messageHandlerChainBuilder,
            int timeInMilliseconds)
        {
            return messageHandlerChainBuilder.AddDecorator(
                currentHandler => new DelayDecorator<TMessageType>(currentHandler, TimeSpan.FromMilliseconds(timeInMilliseconds)).HandleMessageAsync);
        }
    }
}